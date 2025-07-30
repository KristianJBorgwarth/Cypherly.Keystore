using Keystore.Application.Abstractions;
using Keystore.Application.Dtos;

namespace Keystore.Application.Features.KeyBundle.Commands.UploadOTPs;

public sealed record UploadOneTimePreKeysCommand : ICommand
{
    public required Guid Id { get; init; }
    public required IReadOnlyCollection<PreKeyDto>  PreKeys { get; init; }
}