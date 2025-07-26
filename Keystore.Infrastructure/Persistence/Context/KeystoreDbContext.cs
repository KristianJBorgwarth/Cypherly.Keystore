using Keystore.Domain.Aggregates;
using Keystore.Domain.Entities;
using Keystore.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Keystore.Infrastructure.Persistence.Context;

public class KeystoreDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<KeyBundle> KeyBundle { get; init; }
    public DbSet<PreKey> OneTimePreKey { get; init; }
    public DbSet<OutboxMessage> OutboxMessage { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(KeystoreDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}