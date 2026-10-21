using Keystore.Domain.Abstractions;
using Keystore.Domain.Common;
using Keystore.Domain.Entities;
using Keystore.Domain.Events;

// ReSharper disable MemberCanBePrivate.Global

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

// ReSharper disable ConvertToPrimaryConstructor

namespace Keystore.Domain.Aggregates;

public sealed class KeyBundle : AggregateRoot
{
    public Guid UserId { get; private init; }
    public Guid AccessKey { get; private init; }
    public byte[] IdentityKey { get; private set; }
    public ushort RegistrationId { get; private set; }
    public int SignedPrekeyId { get; private set; }
    public byte[] SignedPreKeyPublic { get; private set; }
    public byte[] SignedPreKeySignature { get; private set; }
    public DateTimeOffset SignedPreKeyTimestamp { get; private set; }
    public int KyberPreKeyId { get; private set; }
    public byte[] KyberPreKeyPublic { get; private set; }
    public byte[] KyberPreKeySignature { get; private set; }

    public const int MaxStoredPreKeys = 200;

    private readonly List<PreKey> _preKeys = [];
    public IReadOnlyCollection<PreKey> PreKeys => _preKeys.AsReadOnly();

    // For EF Core
    private KeyBundle() : base(Guid.Empty) { }

    public KeyBundle(
        Guid id,
        Guid userId,
        Guid accessKey,
        byte[] identityKey,
        ushort registrationId,
        int signedPrekeyId,
        byte[] signedPreKeyPublic,
        byte[] signedPreKeySignature,
        DateTimeOffset signedPreKeyTimestamp,
        int kyberPreKeyId,
        byte[] kyberPreKeyPublic,
        byte[] kyberPreKeySignature
    ) : base(id)
    {
        UserId = userId;
        AccessKey = accessKey;
        IdentityKey = identityKey ?? throw new ArgumentNullException(nameof(identityKey));
        RegistrationId = registrationId;
        SignedPreKeyTimestamp = signedPreKeyTimestamp.UtcDateTime;
        RotateSignedPreKey(signedPrekeyId, signedPreKeyPublic, signedPreKeySignature);
        RotateKyberPreKey(kyberPreKeyId, kyberPreKeyPublic, kyberPreKeySignature);
    }

    public void RotateSignedPreKey(int keyId, byte[] pub, byte[] sig)
    {
        SignedPrekeyId = keyId;
        SignedPreKeyPublic = pub ?? throw new ArgumentNullException(nameof(pub));
        SignedPreKeySignature = sig ?? throw new ArgumentNullException(nameof(sig));
    }

    /// <summary>
    /// Last-resort Kyber (PQXDH) prekey. It is served with every bundle and never consumed.
    /// </summary>
    public void RotateKyberPreKey(int keyId, byte[] pub, byte[] sig)
    {
        KyberPreKeyId = keyId;
        KyberPreKeyPublic = pub ?? throw new ArgumentNullException(nameof(pub));
        KyberPreKeySignature = sig ?? throw new ArgumentNullException(nameof(sig));
    }

    public Result UploadPreKeys(IReadOnlyCollection<PreKey> preKeys)
    {
        if (_preKeys.Count(k => k.Consumed is false) + preKeys.Count > MaxStoredPreKeys)
            return Result.Fail(Error.Validation($"A device can store at most {MaxStoredPreKeys} unused pre-keys."));

        if (preKeys.Any(k => _preKeys.Any(stored => stored.KeyId == k.KeyId)))
            return Result.Fail(Error.Validation("A pre-key with the same KeyId is already stored."));

        _preKeys.AddRange(preKeys);
        return Result.Ok();
    }

    public PreKey? ConsumePreKey()
    {
        var pk = _preKeys.FirstOrDefault(k=> k.Consumed is false);
        if (pk is null) return null;

        pk.Consume();
        return pk;
    }
}
