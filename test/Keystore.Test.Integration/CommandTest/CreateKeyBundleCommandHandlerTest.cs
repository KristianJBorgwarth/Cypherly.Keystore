using AutoFixture;
using FluentAssertions;
using Keystore.Application.Abstractions;
using Keystore.Application.Contracts;
using Keystore.Application.Features.KeyBundle.Commands.Create;
using Keystore.Domain.Common;
using Keystore.Infrastructure.Persistence.Context;
using Keystore.Test.Integration.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Keystore.Test.Integration.CommandTest;

public class CreateKeyBundleCommandHandlerTest : IntegrationTestBase
{
    private readonly CreateKeyBundleCommandHandler _sut;
    private readonly Fixture _fixture = new();
    public CreateKeyBundleCommandHandlerTest(IntegrationTestFactory<Program, KeystoreDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        _sut = new CreateKeyBundleCommandHandler(
            scope.ServiceProvider.GetRequiredService<IKeyBundleRepository>(),
            scope.ServiceProvider.GetRequiredService<IUnitOfWork>());
    }

    private CreateKeyBundleCommand CreateCommand(Guid accessKey, Guid connectionId) => _fixture.Build<CreateKeyBundleCommand>()
            .With(x => x.DeviceId, Guid.NewGuid())
            .With(x => x.TenantId, Guid.NewGuid())
            .With(x => x.AccessKey, accessKey)
            .With(x => x.ConnectionId, connectionId)
            .With(x => x.IdentityKey, [1, 2, 3])
            .With(x => x.RegistrationId, 1)
            .With(x => x.SignedPrekeyId, 1)
            .With(x => x.SignedPreKeyPublic, [4, 5, 6])
            .With(x => x.SignedPreKeySignature, [7, 8, 9])
            .With(x => x.SignedPreKeyTimestamp, DateTimeOffset.UtcNow)
            .With(x => x.KyberPreKeyId, 1)
            .With(x => x.KyberPreKeyPublic, [10, 11, 12])
            .With(x => x.KyberPreKeySignature, [13, 14, 15])
            .Create();

    [Fact]
    public async Task CreateKeyBundleCommandHandler_ShouldCreateKeyBundle()
    {
        // Arrange
        var connectionId = Guid.NewGuid();
        var command = CreateCommand(connectionId, connectionId);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Error.Should().BeNull();
        var dbResult = await Db.KeyBundle
            .Include(x => x.PreKeys)
            .FirstOrDefaultAsync(x => x.Id == command.DeviceId);
        dbResult.Should().NotBeNull();
        dbResult!.PreKeys.Count.Should().Be(command.PreKeys.Count);
        dbResult.Id.Should().Be(command.DeviceId);
        dbResult.UserId.Should().Be(command.TenantId);
        dbResult.IdentityKey.Should().BeEquivalentTo(command.IdentityKey);
        dbResult.AccessKey.Should().Be(command.AccessKey);
        dbResult.RegistrationId.Should().Be(command.RegistrationId);
        dbResult.SignedPrekeyId.Should().Be(command.SignedPrekeyId);
        dbResult.SignedPreKeyPublic.Should().BeEquivalentTo(command.SignedPreKeyPublic);
        dbResult.SignedPreKeySignature.Should().BeEquivalentTo(command.SignedPreKeySignature);
        dbResult.KyberPreKeyId.Should().Be(command.KyberPreKeyId);
        dbResult.KyberPreKeyPublic.Should().BeEquivalentTo(command.KyberPreKeyPublic);
        dbResult.KyberPreKeySignature.Should().BeEquivalentTo(command.KyberPreKeySignature);
        dbResult.SignedPreKeyTimestamp.Should().BeCloseTo(command.SignedPreKeyTimestamp, TimeSpan.FromTicks(20));
    }

    [Fact]
    public async Task CreateKeyBundleCommandHandler_Given_AccessKeyNotMatchingConnectionId_Should_RejectAndNotPersist()
    {
        // Arrange
        var command = CreateCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.Unauthorized);
        (await Db.KeyBundle.AnyAsync(x => x.Id == command.DeviceId)).Should().BeFalse();
    }
}
