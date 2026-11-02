using SecureEDM.Client.Contracts.Interfaces;

namespace SecureEDM.Client.Contracts.DTOs.PrivateKey;

public class QueryPrivateKey : IPagination
{
    public List<int>? Id { get; set; }
    public List<int>? AsymmetricKeyId { get; set; }
}