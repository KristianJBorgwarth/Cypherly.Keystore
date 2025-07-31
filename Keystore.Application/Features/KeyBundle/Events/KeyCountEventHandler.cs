using Cypherly.Message.Contracts.Abstractions;
using Cypherly.Message.Contracts.Messages.KeyBundle;
using Keystore.Application.Abstractions;
using Keystore.Application.Contracts;
using Keystore.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Keystore.Application.Features.KeyBundle.Events;

public sealed class KeyCountEventHandler(
    IKeyBundleRepository keyBundleRepository,
    IProducer<KeyCountLowMessage> producer,
    ILogger<KeyCountEventHandler> logger)
    : IDomainEventHandler<KeyCountEvent>
{
    public async Task Handle(KeyCountEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var keyBundle = await keyBundleRepository.GetByIdWithPreKeysAsync(notification.TenantId, cancellationToken);
            
            if (keyBundle is null)
            {
                throw new NullReferenceException($"No key bundle found for tenant id: {notification.TenantId}");
            }

            if (keyBundle.PreKeys.Count > 20) return;

            var message = new KeyCountLowMessage
            {
                ConnectionId = notification.ConnectionId,
                TenantId = notification.TenantId,
                CorrelationId = Guid.NewGuid(),
            };

            await producer.PublishMessageAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "An exception occured while trying to process key count low event");
            throw;
        }
    }
}