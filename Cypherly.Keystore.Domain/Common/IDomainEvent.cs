using MediatR;

namespace Cypherly.Keystore.Domain.Common;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}