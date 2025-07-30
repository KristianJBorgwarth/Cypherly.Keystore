using AutoFixture;
using FluentAssertions;
using Keystore.Application.Abstractions;
using Keystore.Application.Contracts;
using Keystore.Application.Dtos;
using Keystore.Application.Features.KeyBundle.Commands.UploadOTPs;
using Keystore.Domain.Aggregates;
using Keystore.Infrastructure.Persistence.Context;
using Keystore.Test.Integration.Setup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Keystore.Test.Integration.CommandTest;

public class UploadOneTimePreKeysCommandHandlerTest : IntegrationTestBase
{
    private readonly UploadOneTimePreKeysCommandHandler _sut;
    
    public UploadOneTimePreKeysCommandHandlerTest(IntegrationTestFactory<Program, KeystoreDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IKeyBundleRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<UploadOneTimePreKeysCommandHandler>>();
        _sut = new UploadOneTimePreKeysCommandHandler(repository, uow, logger);
    }

    [Fact]
    public async Task Handle_UploadOTPs_Should_Persist_AndReturn_ResultOk()
    {
        // Arrange
        var keyBundle = Fixture.Build<KeyBundle>().Create();
        var preKeys = Fixture.CreateMany<PreKeyDto>(100).ToArray();
        await Db.KeyBundle.AddAsync(keyBundle);
        await Db.SaveChangesAsync();
        
        var cmd = new UploadOneTimePreKeysCommand
        {
            Id = keyBundle.Id,
            PreKeys = preKeys
        };
        
        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);
        
        // Assert
        result.Success.Should().BeTrue();
        Db.OneTimePreKey.Count().Should().Be(100);
    }

    [Fact]
    public async Task Handle_UploadOtps_NoKeyBundle_Should_Return_ResultFail()
    {
        // Arrange
        var preKeys = Fixture.CreateMany<PreKeyDto>(100).ToArray();
        
        var cmd = new UploadOneTimePreKeysCommand
        {
            Id = Guid.NewGuid(),
            PreKeys = preKeys
        };
        
        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);
        
        // Assert
        result.Success.Should().BeFalse();
        Db.OneTimePreKey.Count().Should().Be(0);
    }
}
