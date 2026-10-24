using Keystore.Domain.Abstractions;
using Keystore.Domain.Aggregates;

#pragma warning disable IDE0290 // Use primary constructor

namespace Keystore.Domain.Entities;

public sealed class PreKey : Entity
{
    public Guid KeyBundleId { get; init; }
    public int KeyId { get; private set; }
    public byte[] PublicKey { get; private set; }
    public KeyBundle? KeyBundle { get; private set; }

    public PreKey(
        Guid id,
        Guid keyBundleId,
        int keyId,
        byte[] publicKey
    ) : base(id)
    {
        KeyBundleId = keyBundleId;
        KeyId = keyId;
        PublicKey = publicKey ?? throw new ArgumentNullException(nameof(publicKey));
    }
}
