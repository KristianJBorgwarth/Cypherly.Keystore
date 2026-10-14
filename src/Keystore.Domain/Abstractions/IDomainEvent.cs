using Mediator;

namespace Keystore.Domain.Abstractions;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}