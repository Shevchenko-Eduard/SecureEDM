using SecureEDM.Client.Contracts.Interfaces;

namespace SecureEDM.Client.Contracts.DTOs.PublicKey;

public class QueryPublicKey : IPagination
{
    public List<int>? Id { get; set; }
    public List<int>? AsymmetricKeyId { get; set; }
    public List<int>? RemoteOwnerId { get; set; }
    public bool HasAttestation { get; set; }
}