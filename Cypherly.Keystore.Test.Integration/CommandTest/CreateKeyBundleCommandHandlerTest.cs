using AutoFixture;
using Cypherly.Keystore.Application.Abstractions;
using Cypherly.Keystore.Application.Contracts;
using Cypherly.Keystore.Application.Features.KeyBundle.Command.Create;
using Cypherly.Keystore.Infrastructure.Persistence.Context;
using Cypherly.Keystore.Test.Integration.Setup;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Cypherly.Keystore.Test.Integration.CommandTest;

public class CreateKeyBundleCommandHandlerTest : IntegrationTestBase
{
    private readonly CreateKeyBundleCommandHandler _sut;
    private readonly Fixture _fixture = new Fixture();
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
        var command = _fixture.Create<CreateKeyBundleCommand>();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Error.Should().BeNull();
        Db.KeyBundle.Count().Should().Be(1);
    }
}