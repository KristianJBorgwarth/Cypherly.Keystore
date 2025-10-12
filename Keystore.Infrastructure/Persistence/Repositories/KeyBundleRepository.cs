using Keystore.Application.Contracts;
using Keystore.Domain.Aggregates;
using Keystore.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Keystore.Infrastructure.Persistence.Repositories;

internal sealed class KeyBundleRepository(KeystoreDbContext context) : IKeyBundleRepository
{
    public async Task<KeyBundle> CreateAsync(KeyBundle entity, CancellationToken cancellationToken = default)
    {
        var entry = await context.KeyBundle.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<KeyBundle?> GetByIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await context.KeyBundle.FirstOrDefaultAsync(x => x.Id == tenantId, cancellationToken);
    }

    public async Task<KeyBundle?> GetByIdWithPreKeysAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await context.KeyBundle.Include(x => x.PreKeys).FirstOrDefaultAsync(x => x.Id == tenantId, cancellationToken);
    }

    public async Task DeleteAsync(KeyBundle entity, CancellationToken cancellationToken = default)
    {
        context.KeyBundle.Remove(entity);
    }

    public async Task<KeyBundle?> GetByAccessIdAsync(Guid accessKey, CancellationToken cancellationToken = default)
    {
        var keyBundle = await context.KeyBundle
            .Include(x => x.PreKeys)
            .FirstOrDefaultAsync(x => x.AccessKey == accessKey, cancellationToken: cancellationToken);

        return keyBundle;
    }

}
