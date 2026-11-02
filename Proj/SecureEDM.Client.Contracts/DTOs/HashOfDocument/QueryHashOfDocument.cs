using SecureEDM.Client.Contracts.Interfaces;
using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Domain.Models.ValueObject;

namespace SecureEDM.Client.Contracts.DTOs.HashOfDocument;

public class QueryHashOfDocument : IPagination
{
    public List<int>? Id { get; set; }
    public List<int>? RemoteAuthorId { get; set; }
    public Range<DateTimeOffset>? CreatedAt { get; set; }
    public Range<DateTimeOffset>? RevokedAt { get; set; }
    public List<EnumRevokedReason>? RevokedReason { get; set; }
    public List<EnumDigestAlgorithm>? HashedAlgorithm { get; set; }
    public bool IsPrimary { get; set; }
}