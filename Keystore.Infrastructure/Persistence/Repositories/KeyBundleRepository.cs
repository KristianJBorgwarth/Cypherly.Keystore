using Keystore.Application.Contracts;
using Keystore.Domain.Aggregates;
using Keystore.Infrastructure.Persistence.Context;

namespace Keystore.Infrastructure.Persistence.Repositories;

internal sealed class KeyBundleRepository(KeystoreDbContext context) : IKeyBundleRepository
{
    public async Task<KeyBundle> CreateAsync(KeyBundle entity, CancellationToken cancellationToken = default)
    {
        var entry = await context.KeyBundle.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public Task<KeyBundle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}