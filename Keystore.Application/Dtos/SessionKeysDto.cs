using Keystore.Domain.Aggregates;
using Keystore.Domain.Entities;

namespace Keystore.Application.Dtos;

public sealed record SessionKeysDto
{
    public required byte[] IdentityKey { get; init; }
    public required ushort RegistrationId { get; init; }
    public required int SignedPrekeyId { get; init; }
    public required byte[] SignedPreKeyPublic { get; init; }
    public required byte[] SignedPreKeySignature { get; init; }
    public required PreKeyDto PreKey { get; init; }

    internal static SessionKeysDto MapToSessionKeysDto(KeyBundle keyBundle, PreKey preKey)
    {
        return new SessionKeysDto()
        {
            IdentityKey = keyBundle.IdentityKey,
            RegistrationId = keyBundle.RegistrationId,
            SignedPrekeyId = keyBundle.SignedPrekeyId,
            SignedPreKeyPublic = keyBundle.SignedPreKeyPublic,
            SignedPreKeySignature = keyBundle.SignedPreKeySignature,
            PreKey = new PreKeyDto
            {
                KeyId = preKey.KeyId,
                PublicKey = preKey.PublicKey,
            }
        };
    }
}