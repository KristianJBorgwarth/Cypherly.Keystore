using AutoFixture;
using Keystore.Infrastructure.Persistence.Context;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

#pragma warning disable CA1816
namespace Keystore.Test.Integration.Setup;

[Collection("KeystoreApplication")]
public class IntegrationTestBase : IDisposable
{
    protected readonly KeystoreDbContext Db;
    protected readonly HttpClient Client;
    protected readonly ITestHarness Harness;
    protected readonly Fixture  Fixture = new();

    public IntegrationTestBase(IntegrationTestFactory<Program, KeystoreDbContext> factory)
    {
        Harness = factory.Services.GetTestHarness();
        var scope = factory.Services.CreateScope();
        Db = scope.ServiceProvider.GetRequiredService<KeystoreDbContext>();
        Db.Database.EnsureCreated();
        Client = factory.CreateClient();
        Harness.Start();
    }

    public void Dispose()
    {
        Db.KeyBundle.ExecuteDelete();
        Db.OneTimePreKey.ExecuteDelete();
        Db.OutboxMessage.ExecuteDelete();
        Harness.Stop();
    }
}