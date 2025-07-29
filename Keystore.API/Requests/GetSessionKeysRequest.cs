namespace Keystore.API.Requests;

internal sealed record GetSessionKeysRequest
{
    public required Guid AccessKey { get; init; }
}