using SecureEDM.Client.Contracts.Interfaces;
using SecureEDM.Client.Domain.Enums;
using SecureEDM.Common.Domain.Models.ValueObject;

namespace SecureEDM.Client.Contracts.DTOs.TrackingDocument;

public class QueryTrackingDocument : IPagination
{
    public List<int>? Id { get; set; }
    public List<string>? Path { get; set; }
    public List<EnumStatusTracking>? StatusTracking { get; set; }
    public Range<DateTimeOffset>? CreatedAt { get; set; }
    public Range<DateTimeOffset>? DeleteAt { get; set; }
    public Range<DateTimeOffset>? LastSync { get; set; }
    public List<string>? FixedExtension { get; set; }
}