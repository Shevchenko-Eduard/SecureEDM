namespace SecureEDM.Client.Contracts.DTOs.TrackingDocument;

public class SoftDeleteTrackingDocument
{
    public int Id { get; set; }
    public DateTimeOffset? DeleteAt { get; set; }
}