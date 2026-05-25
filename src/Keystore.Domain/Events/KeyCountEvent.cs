using Keystore.Domain.Abstractions;

namespace Keystore.Domain.Events;

public sealed record KeyCountEvent : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public required Guid TenantId { get; init; }
    public required Guid ConnectionId { get; init; }
}