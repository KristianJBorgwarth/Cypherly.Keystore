using Keystore.Domain.Abstractions;
using MediatR;

namespace Keystore.Application.Abstractions;

public interface IDomainEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent> where TDomainEvent : IDomainEvent { }