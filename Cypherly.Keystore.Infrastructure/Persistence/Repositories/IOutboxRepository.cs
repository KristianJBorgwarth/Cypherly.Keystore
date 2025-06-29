using Cypherly.Keystore.Infrastructure.Persistence.Outbox;

namespace Cypherly.Keystore.Infrastructure.Persistence.Repositories;

internal interface IOutboxRepository
{
    Task<OutboxMessage[]> GetUnprocessedAsync(int batchSize);
    Task MarkAsProcessedAsync(OutboxMessage message);
}