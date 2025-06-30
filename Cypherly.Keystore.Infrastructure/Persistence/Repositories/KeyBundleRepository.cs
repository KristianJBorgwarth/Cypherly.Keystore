using Cypherly.Keystore.Application.Contracts;
using Cypherly.Keystore.Domain.Aggregates;
using Cypherly.Keystore.Infrastructure.Persistence.Context;

namespace Cypherly.Keystore.Infrastructure.Persistence.Repositories;

sealed internal class KeyBundleRepository(KeystoreDbContext context) : IKeyBundleRepository
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