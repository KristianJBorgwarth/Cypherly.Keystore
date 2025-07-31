namespace Keystore.Application.Features.KeyBundle.Queries.GetPreKeyCount;

public sealed record GetPreKeyCountDto
{
    public required int KeyCount { get; init; }
}