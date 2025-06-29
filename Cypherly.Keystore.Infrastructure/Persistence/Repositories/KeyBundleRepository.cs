using Cypherly.Keystore.Application.Contracts;
using Cypherly.Keystore.Domain.Aggregates;

namespace Cypherly.Keystore.Infrastructure.Persistence.Repositories;

sealed internal class KeyBundleRepository : IKeyBundleRepository
{
    public Task<KeyBundle> CreateAsync(KeyBundle entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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