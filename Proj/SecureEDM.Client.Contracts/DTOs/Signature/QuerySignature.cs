using SecureEDM.Client.Contracts.Interfaces;
using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Domain.Models.ValueObject;

namespace SecureEDM.Client.Contracts.DTOs.Signature;

public class QuerySignature : IPagination
{
    public List<int>? Id { get; set; }
    public List<int>? RemoteAuthorId { get; set; }
    public List<int>? DocumentId { get; set; }
    public List<int>? PublicKeyId { get; set; }
    public Range<DateTimeOffset>? SignedAt { get; set; }
    public Range<DateTimeOffset>? RevokedAt { get; set; }
    public List<EnumRevokedReason>? RevokedReason { get; set; }
    public List<EnumSignedAlgorithm>? SignedAlgorithm { get; set; }
}