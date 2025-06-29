using MediatR;

namespace Cypherly.Keystore.Domain.Abstractions;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}