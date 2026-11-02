using SecureEDM.Client.Domain.Enums;
using SecureEDM.Common.Domain.Interfaces;

namespace SecureEDM.Client.Contracts.DTOs.TrackingDocument;

public class CreateTrackingDocument(IClock clock)
{
    protected readonly IClock _clock = clock;
    public string Path { get; set; } = null!;
    public EnumStatusTracking? StatusTracking { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? LastSync { get; set; }
    public string FixedExtension { get; set; } = null!;
}