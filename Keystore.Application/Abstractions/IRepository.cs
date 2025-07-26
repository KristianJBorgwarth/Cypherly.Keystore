using Keystore.Domain.Abstractions;

namespace Keystore.Application.Abstractions;

public interface IRepository<TEntity> where TEntity : AggregateRoot
{
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}