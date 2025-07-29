using Keystore.Application.Abstractions;
using Keystore.Application.Dtos;

namespace Keystore.Application.Features.KeyBundle.Queries.GetPrekey;

public sealed record GetSessionKeysQuery : IQuery<SessionKeysDto>
{
    public Guid AccessId { get; init; }
}