
using Cypherly.Keystore.Application.Dtos;
using Cypherly.Keystore.Application.Features.KeyBundle.Command.Create;

namespace Cypherly.Keystore.API.Requests;

sealed internal class CreateKeyBundleRequest
{
    public required Guid AccessKey { get; init; }
    public required byte[] IdentityKey { get; init; }
    public required ushort RegistrationId { get; init; }
    public required int SignedPrekeyId { get; init; }
    public required byte[] SignedPreKeyPublic { get; init; }
    public required byte[] SignedPreKeySignature { get; init; }
    public required IReadOnlyCollection<PreKeyDto> PreKeys { get; init; }
    public required DateTimeOffset SignedPreKeyTimestamp { get; init; }

    public CreateKeyBundleCommand MapToCommand(Guid userId) => new()
    {
        UserId = userId,
        AccessKey = AccessKey,
        IdentityKey = IdentityKey,
        RegistrationId = RegistrationId,
        SignedPrekeyId = SignedPrekeyId,
        SignedPreKeyPublic = SignedPreKeyPublic,
        SignedPreKeySignature = SignedPreKeySignature,
        SignedPreKeyTimestamp = SignedPreKeyTimestamp
    };
}