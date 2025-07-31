using AutoFixture;
using Cypherly.Message.Contracts.Abstractions;
using Cypherly.Message.Contracts.Messages.KeyBundle;
using FluentAssertions;
using Keystore.Application.Contracts;
using Keystore.Application.Features.KeyBundle.Events;
using Keystore.Domain.Aggregates;
using Keystore.Domain.Entities;
using Keystore.Domain.Events;
using Keystore.Infrastructure.Persistence.Context;
using Keystore.Test.Integration.Setup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Keystore.Test.Integration.EventHandler;

public class KeyCountEventHandlerTest : IntegrationTestBase
{
    private readonly KeyCountEventHandler _sut;
    
    public KeyCountEventHandlerTest(IntegrationTestFactory<Program, KeystoreDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IKeyBundleRepository>();
        var producer = scope.ServiceProvider.GetRequiredService<IProducer<KeyCountLowMessage>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<KeyCountEventHandler>>();
        _sut = new KeyCountEventHandler(repository, producer, logger);
    }

    [Fact]
    public async Task Handle_KeyCountEvent_Should_SendMessage_IfCount_Low()
    {
        // Arrange
        var keyBundle = Fixture.Build<KeyBundle>().Create();
        var preKeys = Fixture.CreateMany<PreKey>(15).ToList();
        
        keyBundle.UploadPreKeys(preKeys);
        await Db.KeyBundle.AddAsync(keyBundle);
        await Db.SaveChangesAsync();

        var keyCountEvent = new KeyCountEvent() { ConnectionId = keyBundle.AccessKey, TenantId = keyBundle.Id };
        
        // Act
        await _sut.Handle(keyCountEvent, CancellationToken.None);
        
        // Assert
        Harness.Published.Select<KeyCountLowMessage>().FirstOrDefault(x =>
            x.Context.Message.ConnectionId == keyBundle.AccessKey && x.Context.Message.TenantId == keyBundle.Id)
            .Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_KeyCountEvent_ShouldNot_SendMessage_If_ValidCount()
    {
        // Arrange
        var keyBundle = Fixture.Build<KeyBundle>().Create();
        var preKeys = Fixture.CreateMany<PreKey>(100).ToList();
        
        keyBundle.UploadPreKeys(preKeys);
        await Db.KeyBundle.AddAsync(keyBundle);
        await Db.SaveChangesAsync();

        var keyCountEvent = new KeyCountEvent() { ConnectionId = keyBundle.AccessKey, TenantId = keyBundle.Id };
        
        // Act
        await _sut.Handle(keyCountEvent, CancellationToken.None);
        
        // Assert
        Harness.Published.Select<KeyCountLowMessage>().FirstOrDefault(x =>
                x.Context.Message.ConnectionId == keyBundle.AccessKey && x.Context.Message.TenantId == keyBundle.Id)
            .Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenKeyBundle_DoesNotExist_Throws_NullReferenceException()
    {
        // Arrange
        var keyCountEvent = new KeyCountEvent() { ConnectionId = Guid.NewGuid(), TenantId = Guid.NewGuid() };
        
        // Act
        var act = async () => await _sut.Handle(keyCountEvent, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NullReferenceException>();
    }
}