using Cypherly.Keystore.Domain.Abstractions;
using Cypherly.Keystore.Domain.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

// ReSharper disable ConvertToPrimaryConstructor

namespace Cypherly.Keystore.Domain.Aggregates;

public sealed class KeyBundle : AggregateRoot
{
    public Guid AccessKey { get; private init; }
    public byte[] IdentityKey { get; private set; }
    public ushort RegistrationId { get; private set; }
    public int SignedPrekeyId { get; private set; }
    public byte[] SignedPreKeyPublic { get; private set; }
    public byte[] SignedPreKeySignature { get; private set; }
    public DateTime SignedPreKeyTimestamp { get; private set; }

    private readonly List<PreKey> _preKeys = [];
    public IReadOnlyCollection<PreKey> PreKeys  => _preKeys.AsReadOnly();

    // For EF Core
    private KeyBundle() : base(Guid.Empty) {}

    public KeyBundle(
        Guid id,
        Guid accessKey,
        byte[] identityKey,
        ushort registrationId,
        int signedPrekeyId,
        byte[] signedPreKeyPublic,
        byte[] signedPreKeySignature,
        DateTimeOffset signedPreKeyTimestamp
    ) : base(id)
    {

        AccessKey = accessKey;
        IdentityKey = identityKey ?? throw new ArgumentNullException(nameof(identityKey));
        RegistrationId = registrationId;
        SignedPreKeyTimestamp = signedPreKeyTimestamp.UtcDateTime;
        RotateSignedPreKey(signedPrekeyId, signedPreKeyPublic, signedPreKeySignature);
    }

    public void RotateSignedPreKey(int keyId, byte[] pub, byte[] sig)
    {
        SignedPrekeyId = keyId;
        SignedPreKeyPublic = pub ?? throw new ArgumentNullException(nameof(pub));
        SignedPreKeySignature = sig ?? throw new ArgumentNullException(nameof(sig));
    }

    public void UploadPreKeys(IReadOnlyCollection<PreKey> prekeys)
    {
        _preKeys.AddRange(prekeys);
    }

    public PreKey? ConsumePreKey(int keyId)
    {
        var pk = _preKeys.Find(k=> k.KeyId == keyId && k.Consumed is false);
        if(pk is null) return null;

        pk.Consume();
        return pk;
    }
}