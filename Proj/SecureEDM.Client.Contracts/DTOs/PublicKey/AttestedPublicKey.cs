using System.Collections.Immutable;

namespace SecureEDM.Client.Contracts.DTOs.PublicKey;

public class AttestedPublicKey
{
    public int Id { get; set; }
    public ImmutableArray<byte>? Attestation { get; set; }
}