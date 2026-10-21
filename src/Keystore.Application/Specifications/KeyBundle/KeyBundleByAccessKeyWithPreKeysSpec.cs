using Keystore.Application.Abstractions;
using Keystore.Domain.Aggregates;

internal sealed class KeyBundleByAccessKeyWithPreKeysSpec : Specification<KeyBundle>
{
    public KeyBundleByAccessKeyWithPreKeysSpec(Guid accessKey) : base(keyBundle => keyBundle.AccessKey == accessKey)
    {
        AddIncludes($"{nameof(KeyBundle.PreKeys)}");
    }
}
