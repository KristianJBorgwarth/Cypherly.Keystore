
using Cypherly.Keystore.Application.Messages;

namespace Cypherly.Keystore.Application.Contracts;

public interface IProducer<in TMessage> where TMessage : BaseMessage
{
    Task PublishMessageAsync(TMessage message, CancellationToken cancellationToken = default);
}