
using Keystore.Application.Dtos;
using Keystore.Application.Features.KeyBundle.Commands.Create;

namespace Keystore.API.Requests;

internal sealed class CreateKeyBundleRequest
{
    public required Guid AccessKey { get; init; }
    public required byte[] IdentityKey { get; init; }
    public required ushort RegistrationId { get; init; }
    public required int SignedPrekeyId { get; init; }
    public required byte[] SignedPreKeyPublic { get; init; }
    public required byte[] SignedPreKeySignature { get; init; }
    public required IReadOnlyCollection<PreKeyDto> PreKeys { get; init; }
    public required DateTimeOffset SignedPreKeyTimestamp { get; init; }

    public CreateKeyBundleCommand MapToCommand(Guid userId, Guid deviceId) => new()
    {
        TenantId = userId,
        DeviceId = deviceId,
        AccessKey = AccessKey,
        IdentityKey = IdentityKey,
        RegistrationId = RegistrationId,
        SignedPrekeyId = SignedPrekeyId,
        SignedPreKeyPublic = SignedPreKeyPublic,
        SignedPreKeySignature = SignedPreKeySignature,
        PreKeys = PreKeys,
        SignedPreKeyTimestamp = SignedPreKeyTimestamp
    };
}
