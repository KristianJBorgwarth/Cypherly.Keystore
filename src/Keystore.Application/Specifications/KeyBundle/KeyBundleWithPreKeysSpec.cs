using Keystore.Application.Abstractions;
using Keystore.Domain.Aggregates;

internal sealed class KeyBundleWithPreKeysSpec : Specification<KeyBundle>
{
    public KeyBundleWithPreKeysSpec(Guid id) : base(keyBundle => keyBundle.Id == id)
    {
        AddIncludes($"{nameof(KeyBundle.PreKeys)}");
    }
}
