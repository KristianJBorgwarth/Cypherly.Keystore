using Cypherly.Keystore.Domain.Aggregates;
using Cypherly.Keystore.Domain.Entities;
using Cypherly.Keystore.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Cypherly.Keystore.Infrastructure.Persistence.Context;

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