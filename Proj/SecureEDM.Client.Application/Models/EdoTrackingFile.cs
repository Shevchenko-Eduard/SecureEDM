using SecureEDM.Client.Domain.Enums;

namespace SecureEDM.Client.Application.Models;

/// <summary>Отслеживаемый файл, соответствующий TrackingDocument из доменной модели клиента.</summary>
public sealed class EdoTrackingFile
{
    public string Id { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string FixedExtension { get; set; } = string.Empty;
    public EnumStatusTracking StatusTracking { get; set; } = EnumStatusTracking.Active;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastSync { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
