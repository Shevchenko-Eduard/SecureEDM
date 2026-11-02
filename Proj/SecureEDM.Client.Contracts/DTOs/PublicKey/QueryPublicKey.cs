using SecureEDM.Client.Contracts.Interfaces;
using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Domain.Models.ValueObject;

namespace SecureEDM.Client.Contracts.DTOs.PublicKey;

public class QueryPublicKey : IPagination
{
    public List<int>? Id { get; set; }
    public List<int>? AsymmetricKeyId { get; set; }
    public List<int>? RemoteOwnerId { get; set; }
    public bool HasAttestation { get; set; }
}