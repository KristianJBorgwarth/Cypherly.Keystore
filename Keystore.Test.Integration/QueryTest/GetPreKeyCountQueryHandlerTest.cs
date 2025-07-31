using AutoFixture;
using FluentAssertions;
using Keystore.Application.Contracts;
using Keystore.Application.Features.KeyBundle.Queries.GetPreKeyCount;
using Keystore.Domain.Aggregates;
using Keystore.Domain.Entities;
using Keystore.Infrastructure.Persistence.Context;
using Keystore.Test.Integration.Setup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Keystore.Test.Integration.QueryTest;

public class GetPreKeyCountQueryHandlerTest : IntegrationTestBase
{
    private readonly GetPreKeyCountQueryHandler _sut;
    
    public GetPreKeyCountQueryHandlerTest(IntegrationTestFactory<Program, KeystoreDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IKeyBundleRepository>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<GetPreKeyCountQueryHandler>>();
        
        _sut = new GetPreKeyCountQueryHandler(repo, logger);
    }

    [Fact]
    public async Task Handle_KeyBundlePresent_ShouldReturn_PrekeyCount()
    {
        // Arrange
        var keyBundle = Fixture.Build<KeyBundle>().Create();
        var preKeys = Fixture.CreateMany<PreKey>(23).ToList();
        keyBundle.UploadPreKeys(preKeys);
        await Db.KeyBundle.AddAsync(keyBundle);
        await Db.SaveChangesAsync();

        var query = new GetPreKeyCountQuery { TenantId = keyBundle.Id };
        
        // Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        // Arrange
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.KeyCount.Should().Be(23);
    }

    [Fact]
    public async Task Handle_KeyBundleNotPresent_ShouldReturn_NotFound()
    {
        // Arrange
        var keyBundle = Fixture.Build<KeyBundle>().Create();
        var preKeys = Fixture.CreateMany<PreKey>(23).ToList();
        keyBundle.UploadPreKeys(preKeys);
        await Db.KeyBundle.AddAsync(keyBundle);
        await Db.SaveChangesAsync();
        
        var query = new GetPreKeyCountQuery { TenantId = Guid.NewGuid() };
        
        // Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        // Arrange
        result.Success.Should().BeFalse();
    }
}