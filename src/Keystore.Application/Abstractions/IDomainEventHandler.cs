using Keystore.Domain.Abstractions;
using Mediator;

namespace Keystore.Application.Abstractions;

public interface IDomainEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent> where TDomainEvent : IDomainEvent { }