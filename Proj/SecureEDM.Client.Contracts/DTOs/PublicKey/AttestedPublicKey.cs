namespace SecureEDM.Client.Contracts.DTOs.PublicKey;

public class AttestedPublicKey
{
    public int Id { get; set; }
    public byte[] Attestation { get; set; } = null!;
}