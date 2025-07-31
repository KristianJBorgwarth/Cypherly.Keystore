using AutoFixture;
using FluentAssertions;
using Keystore.Application.Abstractions;
using Keystore.Application.Contracts;
using Keystore.Application.Features.KeyBundle.Queries.GetPrekey;
using Keystore.Domain.Aggregates;
using Keystore.Domain.Common;
using Keystore.Domain.Entities;
using Keystore.Infrastructure.Persistence.Context;
using Keystore.Test.Integration.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Keystore.Test.Integration.QueryTest;

public class SessionKeysQueryHandlerTest : IntegrationTestBase
{
    private readonly GetSessionKeysQueryHandler _sut;
    
    public SessionKeysQueryHandlerTest(IntegrationTestFactory<Program, KeystoreDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IKeyBundleRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<GetSessionKeysQueryHandler>>(); 
        _sut = new GetSessionKeysQueryHandler(repo, uow, logger);
    }

    [Fact]
    public async Task Handle_GivenValidRequest_ShouldReturn_SessionKeysDto()
    {
        // Arrange
        var keyBundle = Fixture.Build<KeyBundle>().Create();
        var preKeys = Fixture.CreateMany<PreKey>(100).ToArray();
        keyBundle.UploadPreKeys(preKeys);
        await Db.KeyBundle.AddAsync(keyBundle);
        await Db.SaveChangesAsync();

        var query = new GetSessionKeysQuery()
        {
            AccessKey = keyBundle.AccessKey
        };
        
        // Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        // Arrange
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.IdentityKey.Should().BeEquivalentTo(keyBundle.IdentityKey);
        result.Value.RegistrationId.Should().Be(keyBundle.RegistrationId);
        result.Value.SignedPrekeyId.Should().Be(keyBundle.SignedPrekeyId);
        result.Value.SignedPreKeyPublic.Should().BeEquivalentTo(keyBundle.SignedPreKeyPublic);
        result.Value.SignedPreKeySignature.Should().BeEquivalentTo(keyBundle.SignedPreKeySignature);
        result.Value.PreKey.Should().NotBeNull();
        Db.OneTimePreKey.AsNoTracking().Where(x => x.Consumed == true).ToList().Should().HaveCount(1);
        var consumedPrekey = Db.OneTimePreKey.AsNoTracking().FirstOrDefault(x => x.Consumed == true);
        consumedPrekey.Should().NotBeNull();
        result.Value.PreKey.KeyId.Should().Be(consumedPrekey.KeyId);
        result.Value.PreKey.PublicKey.Should().BeEquivalentTo(consumedPrekey.PublicKey);
    }

    [Fact]
    public async Task Handle_GivenInvalidRequest_ShouldReturn_ResultFail()
    {
        // Arrange
        var keyBundle = Fixture.Build<KeyBundle>().Create();
        var preKeys = Fixture.CreateMany<PreKey>(100).ToArray();
        keyBundle.UploadPreKeys(preKeys);
        await Db.KeyBundle.AddAsync(keyBundle);
        await Db.SaveChangesAsync();

        var query = new GetSessionKeysQuery()
        {
            AccessKey = Guid.NewGuid()
        };
        
        // Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        // Arrange
        result.Success.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCodes.KeyNotFound);
        Db.OneTimePreKey.AsNoTracking().Where(x => x.Consumed == true).ToList().Should().HaveCount(0);
    }

    [Fact]
    public async Task Handle_GivenNoOneTimePreKeys_ShouldReturn_Empty_OTP()
    {
        // Arrange
        var keyBundle = Fixture.Build<KeyBundle>().Create();
        await Db.KeyBundle.AddAsync(keyBundle);
        await Db.SaveChangesAsync();

        var query = new GetSessionKeysQuery()
        {
            AccessKey = keyBundle.AccessKey
        };
        
        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.IdentityKey.Should().BeEquivalentTo(keyBundle.IdentityKey);
        result.Value.RegistrationId.Should().Be(keyBundle.RegistrationId);
        result.Value.SignedPrekeyId.Should().Be(keyBundle.SignedPrekeyId);
        result.Value.SignedPreKeyPublic.Should().BeEquivalentTo(keyBundle.SignedPreKeyPublic);
        result.Value.SignedPreKeySignature.Should().BeEquivalentTo(keyBundle.SignedPreKeySignature);
        result.Value.PreKey.Should().BeNull();
        Db.OutboxMessage.AsNoTracking().FirstOrDefault().Should().NotBeNull();
    }
}