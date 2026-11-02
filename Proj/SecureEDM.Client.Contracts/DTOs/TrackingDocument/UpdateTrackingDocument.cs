using System.Collections.Immutable;
using SecureEDM.Client.Domain.Enums;
using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Domain.Interfaces;

namespace SecureEDM.Client.Contracts.DTOs.TrackingDocument;

public class UpdateTrackingDocument
{
    public int Id { get; set; }
    public string Path { get; set; } = null!;
    public EnumStatusTracking? StatusTracking { get; set; }
    public DateTimeOffset? LastSync { get; set; }
}