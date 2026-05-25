using Keystore.Domain.Abstractions;

namespace Keystore.Application.Abstractions;

public interface IRepository<TEntity> where TEntity : AggregateRoot
{
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}
