using Keystore.Infrastructure.Persistence.Outbox;

namespace Keystore.Infrastructure.Persistence.Repositories;

internal interface IOutboxRepository
{
    Task<OutboxMessage[]> GetUnprocessedAsync(int batchSize);
    Task MarkAsProcessedAsync(OutboxMessage message);
}