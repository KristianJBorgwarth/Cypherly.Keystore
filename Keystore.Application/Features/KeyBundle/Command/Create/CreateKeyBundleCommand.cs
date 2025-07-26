using Keystore.Application.Abstractions;

namespace Keystore.Application.Features.KeyBundle.Command.Create;

public sealed record CreateKeyBundleCommand : ICommand
{
    public required Guid UserId { get; init; }
    public required Guid AccessKey { get; init; }
    public required byte[] IdentityKey { get; init; }
    public required ushort RegistrationId { get; init; }
    public required int SignedPrekeyId { get; init; }
    public required byte[] SignedPreKeyPublic { get; init; }
    public required byte[] SignedPreKeySignature { get; init; }
    public required DateTimeOffset SignedPreKeyTimestamp { get; init; }
}