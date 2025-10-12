using Keystore.Application.Abstractions;

namespace Keystore.Application.Features.KeyBundle.Queries.GetPreKeyCount;

public sealed record GetPreKeyCountQuery : IQuery<GetPreKeyCountDto>
{
    public required Guid Id { get; init; }
}
