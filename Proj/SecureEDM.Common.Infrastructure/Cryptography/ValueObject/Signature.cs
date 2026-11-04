using System.Collections.Immutable;
using SecureEDM.Common.Domain.Enums;

namespace SecureEDM.Common.Infrastructure.Cryptography.ValueObject;

public class Signature(
    EnumSignedAlgorithm algorithm,
    byte[] signature,
    byte[] publicKey
)
{
    public EnumSignedAlgorithm Algorithm { get; init; } = algorithm;
    public ImmutableArray<byte> Value { get; init; } = [.. signature];
    public ImmutableArray<byte> PublicKey { get; init; } = [.. publicKey];
}