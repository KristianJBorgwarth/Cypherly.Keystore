using Keystore.Application.Dtos;

namespace Keystore.API.Requests;

public sealed record UploadOneTimePreKeysRequest
{
    public required IReadOnlyCollection<PreKeyDto>  PreKeys { get; init; }
}