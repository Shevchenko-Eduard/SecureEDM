using System.Collections.Immutable;

namespace SecureEDM.Common.Domain.Models.ValueObject;

public class KeyPair(
    byte[] publicKey, 
    byte[] privateKey)
{
    public ImmutableArray<byte> PublicKey { get; init; } = [.. publicKey];
    public ImmutableArray<byte> PrivateKey { get; init; } = [.. privateKey];

}