using AutoFixture;
using FluentAssertions;
using Keystore.Application.Abstractions;
using Keystore.Application.Contracts;
using Keystore.Application.Exceptions;
using Keystore.Application.Features.KeyBundle.Queries.GetPrekey;
using Keystore.Domain.Aggregates;
using Keystore.Domain.Common;
using Keystore.Domain.Entities;
using Keystore.Infrastructure.Persistence.Context;
using Keystore.Test.Integration.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Keystore.Test.Integration.QueryTest;

public class SessionKeysQueryHandlerTest : IntegrationTestBase
{
    private readonly GetSessionKeysQueryHandler _sut;
    private readonly IntegrationTestFactory<Program, KeystoreDbContext> _factory;
    
    public SessionKeysQueryHandlerTest(IntegrationTestFactory<Program, KeystoreDbContext> factory) : base(factory)
    {
        _factory = factory;
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IKeyBundleRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        _sut = new GetSessionKeysQueryHandler(repo, uow);
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
        result.Value.KyberPreKeyId.Should().Be(keyBundle.KyberPreKeyId);
        result.Value.KyberPreKeyPublic.Should().BeEquivalentTo(keyBundle.KyberPreKeyPublic);
        result.Value.KyberPreKeySignature.Should().BeEquivalentTo(keyBundle.KyberPreKeySignature);
        result.Value.PreKey.Should().NotBeNull();
        Db.OneTimePreKey.AsNoTracking().Should().HaveCount(99);
        Db.OneTimePreKey.AsNoTracking().Any(x => x.KeyId == result.Value.PreKey!.KeyId).Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Given_Two_Fetches_Should_Hand_Out_Different_PreKeys_And_Delete_Both()
    {
        // Arrange
        var keyBundle = Fixture.Build<KeyBundle>().Create();
        keyBundle.UploadPreKeys(Fixture.CreateMany<PreKey>(10).ToArray());
        await Db.KeyBundle.AddAsync(keyBundle);
        await Db.SaveChangesAsync();

        var query = new GetSessionKeysQuery { AccessKey = keyBundle.AccessKey };

        // Act
        var first = await _sut.Handle(query, CancellationToken.None);
        var second = await _sut.Handle(query, CancellationToken.None);

        // Assert
        first.Value!.PreKey!.KeyId.Should().NotBe(second.Value!.PreKey!.KeyId);
        Db.OneTimePreKey.AsNoTracking().Should().HaveCount(8);
    }

    [Fact]
    public async Task Consuming_The_Same_PreKey_From_Two_Contexts_Should_Let_Only_One_Succeed()
    {
        // Arrange
        var keyBundle = Fixture.Build<KeyBundle>().Create();
        keyBundle.UploadPreKeys(Fixture.CreateMany<PreKey>(1).ToArray());
        await Db.KeyBundle.AddAsync(keyBundle);
        await Db.SaveChangesAsync();

        using var scopeA = _factory.Services.CreateScope();
        using var scopeB = _factory.Services.CreateScope();
        var bundleA = await scopeA.ServiceProvider.GetRequiredService<KeystoreDbContext>().KeyBundle.Include(k => k.PreKeys).FirstAsync(k => k.Id == keyBundle.Id);
        var bundleB = await scopeB.ServiceProvider.GetRequiredService<KeystoreDbContext>().KeyBundle.Include(k => k.PreKeys).FirstAsync(k => k.Id == keyBundle.Id);

        var preKeyA = bundleA.ConsumePreKey();
        var preKeyB = bundleB.ConsumePreKey();
        preKeyA!.KeyId.Should().Be(preKeyB!.KeyId);

        // Act
        await scopeA.ServiceProvider.GetRequiredService<IUnitOfWork>().SaveChangesAsync();
        var second = () => scopeB.ServiceProvider.GetRequiredService<IUnitOfWork>().SaveChangesAsync();

        // Assert
        await second.Should().ThrowAsync<ConcurrencyConflictException>();
        Db.OneTimePreKey.AsNoTracking().Should().BeEmpty();
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
        result.Error!.Code.Should().Be("entity.not.found");
        Db.OneTimePreKey.AsNoTracking().Should().HaveCount(100);
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

    [Fact]
    public async Task Handle_Given_PreKey_Taken_By_Another_Request_Should_Return_Conflict()
    {
        // Arrange
        var keyBundle = Fixture.Build<KeyBundle>().Create();
        keyBundle.UploadPreKeys(Fixture.CreateMany<PreKey>(1).ToArray());
        await Db.KeyBundle.AddAsync(keyBundle);
        await Db.SaveChangesAsync();

        using var loserScope = _factory.Services.CreateScope();
        await loserScope.ServiceProvider.GetRequiredService<KeystoreDbContext>().KeyBundle.Include(k => k.PreKeys).FirstAsync(k => k.Id == keyBundle.Id);
        var loser = new GetSessionKeysQueryHandler(
            loserScope.ServiceProvider.GetRequiredService<IKeyBundleRepository>(),
            loserScope.ServiceProvider.GetRequiredService<IUnitOfWork>());

        var winner = await _sut.Handle(new GetSessionKeysQuery { AccessKey = keyBundle.AccessKey }, CancellationToken.None);

        // Act
        var result = await loser.Handle(new GetSessionKeysQuery { AccessKey = keyBundle.AccessKey }, CancellationToken.None);

        // Assert
        winner.Success.Should().BeTrue();
        result.Success.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.Conflict);
        Db.OneTimePreKey.AsNoTracking().Should().BeEmpty();
    }
}
