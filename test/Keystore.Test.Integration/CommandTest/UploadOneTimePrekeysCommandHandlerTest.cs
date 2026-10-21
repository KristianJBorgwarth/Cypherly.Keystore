using AutoFixture;
using FluentAssertions;
using Keystore.Application.Abstractions;
using Keystore.Application.Contracts;
using Keystore.Application.Dtos;
using Keystore.Application.Features.KeyBundle.Commands.UploadOTPs;
using Keystore.Domain.Aggregates;
using Keystore.Domain.Entities;
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
        _sut = new UploadOneTimePreKeysCommandHandler(repository, uow);
    }

    private static PreKeyDto[] PreKeyDtos(int firstKeyId, int count) =>
        [.. Enumerable.Range(firstKeyId, count).Select(id => new PreKeyDto { KeyId = id, PublicKey = [0x05, .. new byte[32]] })];

    private async Task<KeyBundle> SeedBundleAsync(int storedPreKeys, int consumed = 0)
    {
        var keyBundle = Fixture.Build<KeyBundle>().Create();
        keyBundle.UploadPreKeys([.. Enumerable.Range(1, storedPreKeys).Select(id => new PreKey(Guid.NewGuid(), keyBundle.Id, id, [0x05, .. new byte[32]]))]);

        for (var i = 0; i < consumed; i++)
            keyBundle.ConsumePreKey();

        await Db.KeyBundle.AddAsync(keyBundle);
        await Db.SaveChangesAsync();
        return keyBundle;
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

    [Fact]
    public async Task Handle_Given_Upload_Up_To_The_Stored_Cap_Should_Succeed()
    {
        // Arrange
        var keyBundle = await SeedBundleAsync(storedPreKeys: 100);

        // Act
        var result = await _sut.Handle(new UploadOneTimePreKeysCommand { Id = keyBundle.Id, PreKeys = PreKeyDtos(101, 100) }, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        Db.OneTimePreKey.Count().Should().Be(KeyBundle.MaxStoredPreKeys);
    }

    [Fact]
    public async Task Handle_Given_Upload_Over_The_Stored_Cap_Should_Fail_And_Store_Nothing()
    {
        // Arrange
        var keyBundle = await SeedBundleAsync(storedPreKeys: 150);

        // Act
        var result = await _sut.Handle(new UploadOneTimePreKeysCommand { Id = keyBundle.Id, PreKeys = PreKeyDtos(151, 51) }, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        Db.OneTimePreKey.Count().Should().Be(150);
    }

    [Fact]
    public async Task Handle_Should_Not_Count_Consumed_PreKeys_Towards_The_Cap()
    {
        // Arrange
        var keyBundle = await SeedBundleAsync(storedPreKeys: 150, consumed: 100);

        // Act
        var result = await _sut.Handle(new UploadOneTimePreKeysCommand { Id = keyBundle.Id, PreKeys = PreKeyDtos(151, 100) }, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Given_KeyId_Already_Stored_Should_Fail_And_Store_Nothing()
    {
        // Arrange
        var keyBundle = await SeedBundleAsync(storedPreKeys: 10);

        // Act
        var result = await _sut.Handle(new UploadOneTimePreKeysCommand { Id = keyBundle.Id, PreKeys = PreKeyDtos(5, 10) }, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        Db.OneTimePreKey.Count().Should().Be(10);
    }
}
