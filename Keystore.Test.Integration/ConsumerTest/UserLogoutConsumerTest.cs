using AutoFixture;
using Cypherly.Message.Contracts.Messages.User;
using FakeItEasy;
using FluentAssertions;
using Keystore.Application.Abstractions;
using Keystore.Application.Contracts;
using Keystore.Application.Features.KeyBundle.Consumers;
using Keystore.Domain.Aggregates;
using Keystore.Infrastructure.Persistence.Context;
using Keystore.Test.Integration.Setup;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Keystore.Test.Integration.ConsumerTest;

public sealed class UserLogoutConsumerTest : IntegrationTestBase
{
    private readonly UserLogoutConsumer _sut;
    private readonly Fixture _fixture = new();

    public UserLogoutConsumerTest(IntegrationTestFactory<Program, KeystoreDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IKeyBundleRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<UserLogoutConsumer>>();
        _sut = new UserLogoutConsumer(repo, uow, logger);
    }


    [Fact]
    public async Task Consume_GivenValidMessage_AndPresentKeyBundle_ShouldDeleteKeyBundle()
    {
        // Arrange
        var keyBundle = new KeyBundle(
            id: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            identityKey: [1, 2, 3],
            accessKey: Guid.NewGuid(),
            registrationId: 1,
            signedPrekeyId: 1,
            signedPreKeyPublic: [4, 5, 6],
            signedPreKeySignature: [7, 8, 9],
            signedPreKeyTimestamp: DateTimeOffset.UtcNow);
        
        keyBundle.UploadPreKeys([.. _fixture.CreateMany<Domain.Entities.PreKey>(5).Select(x =>
            new Domain.Entities.PreKey(
                id: Guid.NewGuid(),
                keyBundleId: keyBundle.Id,
                keyId: x.KeyId,
                publicKey: x.PublicKey))]);

        await Db.KeyBundle.AddAsync(keyBundle);
        await Db.SaveChangesAsync();

        var message = new UserLogoutMessage
        {
            ConnectionId = Guid.NewGuid(),
            DeviceId = keyBundle.Id,
            UserId = keyBundle.UserId,
            CorrelationId = Guid.NewGuid()
        };

        var fakeConsumeContext = A.Fake<ConsumeContext<UserLogoutMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(message);
        
        // Act
        await _sut.Consume(fakeConsumeContext);

        // Assert
        var dbResult = await Db.KeyBundle.AsNoTracking().FirstOrDefaultAsync(x => x.Id == keyBundle.Id);
        dbResult.Should().BeNull();
        Db.OneTimePreKey.Should().HaveCount(0);
    }
}
