using SecureEDM.Client.Domain.Enums;

namespace SecureEDM.Client.Contracts.DTOs.TrackingDocument;

public class UpdateTrackingDocument
{
    public int Id { get; set; }
    public string Path { get; set; } = null!;
    public EnumStatusTracking? StatusTracking { get; set; }
    public DateTimeOffset? LastSync { get; set; }
}