namespace Keystore.Application.Dtos;

public sealed class PreKeyDto
{
    public int KeyId { get; init; }
    public byte[] PublicKey { get; init; } = null!;
}