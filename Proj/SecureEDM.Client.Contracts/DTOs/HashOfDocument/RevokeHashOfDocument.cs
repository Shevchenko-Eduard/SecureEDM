using SecureEDM.Common.Domain.Enums;

namespace SecureEDM.Client.Contracts.DTOs.HashOfDocument;

public class RevokeHashOfDocument
{
    public int Id { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public EnumRevokedReason RevokedReason { get; set; }

}