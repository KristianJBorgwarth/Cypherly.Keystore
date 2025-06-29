﻿using Cypherly.Keystore.Application.Contracts;
using Cypherly.Keystore.Application.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Cypherly.Keystore.Infrastructure.Messaging;

public sealed class Producer<TMessage>(
    IPublishEndpoint publishEndpoint,
    ILogger<Producer<TMessage>> logger)
    : IProducer<TMessage>
    where TMessage : BaseMessage
{

    public async Task PublishMessageAsync(TMessage message, CancellationToken cancellationToken)
    {
        try
        {
            await publishEndpoint.Publish(message, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in {Producer} for message with id: {Id}",
                nameof(Producer<TMessage>),
                message.Id);
            throw;
        }
    }
}