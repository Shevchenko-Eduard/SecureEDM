using System.Collections.Immutable;
using SecureEDM.Common.Domain.Enums;

namespace SecureEDM.Common.Domain.Models.ValueObject;

public class CryptoSignature(
    EnumSignedAlgorithm algorithm,
    byte[] signature,
    byte[] publicKey
)
{
    public EnumSignedAlgorithm Algorithm { get; init; } = algorithm;
    public ImmutableArray<byte> Value { get; init; } = [.. signature];
    public ImmutableArray<byte> PublicKey { get; init; } = [.. publicKey];
}