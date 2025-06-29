using Cypherly.Keystore.Application.Abstractions;

namespace Cypherly.Keystore.Infrastructure.Persistence.Repositories;

 sealed internal class UnitOfWork : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}