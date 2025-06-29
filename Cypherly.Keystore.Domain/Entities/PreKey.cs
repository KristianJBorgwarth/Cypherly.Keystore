using Cypherly.Keystore.Domain.Abstractions;
using Cypherly.Keystore.Domain.Aggregates;

// ReSharper disable ConvertToPrimaryConstructor

namespace Cypherly.Keystore.Domain.Entities;

public sealed class PreKey : Entity
{
    public Guid KeyBundleId { get; init; }
    public int KeyId { get; private set; }
    public byte[] PublicKey { get; private set; }
    public bool Consumed { get; private set; }
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
        Consumed = false;
    }

    public void Consume()
    {
        if (Consumed)
            throw new InvalidOperationException("This prekey has already been consumed.");

        Consumed = true;
    }
}