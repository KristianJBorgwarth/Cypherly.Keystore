using AutoFixture;
using FluentAssertions;
using Keystore.Application.Abstractions;
using Keystore.Application.Contracts;
using Keystore.Application.Features.KeyBundle.Commands.Create;
using Keystore.Infrastructure.Persistence.Context;
using Keystore.Test.Integration.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
            scope.ServiceProvider.GetRequiredService<IUnitOfWork>(),
            scope.ServiceProvider.GetRequiredService<ILogger<CreateKeyBundleCommandHandler>>());
    }

    [Fact]
    public async Task CreateKeyBundleCommandHandler_ShouldCreateKeyBundle()
    {
        // Arrange
        var command = _fixture.Build<CreateKeyBundleCommand>()
            .With(x => x.DeviceId, Guid.NewGuid())
            .With(x => x.TenantId, Guid.NewGuid())
            .With(x => x.AccessKey, Guid.NewGuid())
            .With(x => x.IdentityKey, [1, 2, 3])
            .With(x => x.RegistrationId, 1)
            .With(x => x.SignedPrekeyId, 1)
            .With(x => x.SignedPreKeyPublic, [4, 5, 6])
            .With(x => x.SignedPreKeySignature, [7, 8, 9])
            .With(x => x.SignedPreKeyTimestamp, DateTimeOffset.UtcNow) 
            .Create();

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
        dbResult.SignedPreKeyTimestamp.Should().BeCloseTo(command.SignedPreKeyTimestamp, TimeSpan.FromTicks(20));
    }
}
