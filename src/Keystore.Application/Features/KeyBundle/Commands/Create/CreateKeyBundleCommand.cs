using Keystore.Application.Abstractions;
using Keystore.Application.Dtos;

namespace Keystore.Application.Features.KeyBundle.Commands.Create;

public sealed record CreateKeyBundleCommand : ICommand
{
    public required Guid TenantId { get; init; }
    public required Guid DeviceId { get; init; }

    /// <summary>
    /// The caller's connection ID, taken from the JWT. The client-supplied <see cref="AccessKey"/> must match it.
    /// </summary>
    public required Guid ConnectionId { get; init; }
    public required Guid AccessKey { get; init; }
    public required byte[] IdentityKey { get; init; }
    public required ushort RegistrationId { get; init; }
    public required int SignedPrekeyId { get; init; }
    public required byte[] SignedPreKeyPublic { get; init; }
    public required byte[] SignedPreKeySignature { get; init; }
    public required IReadOnlyCollection<PreKeyDto> PreKeys { get; init; } = [];
    public required DateTimeOffset SignedPreKeyTimestamp { get; init; }
    public required int KyberPreKeyId { get; init; }
    public required byte[] KyberPreKeyPublic { get; init; }
    public required byte[] KyberPreKeySignature { get; init; }
}
