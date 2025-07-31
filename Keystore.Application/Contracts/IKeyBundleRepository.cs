using Keystore.Domain.Aggregates;
using Keystore.Application.Abstractions;

namespace Keystore.Application.Contracts;

public interface IKeyBundleRepository : IRepository<KeyBundle>
{
    Task<KeyBundle?> GetByAccessIdAsync(Guid accessKey, CancellationToken cancellationToken = default);
    Task<KeyBundle?> GetByIdWithPreKeysAsync(Guid tenantId, CancellationToken cancellationToken = default);
}