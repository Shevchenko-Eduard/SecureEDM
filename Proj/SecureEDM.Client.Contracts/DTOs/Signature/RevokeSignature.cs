using SecureEDM.Common.Domain.Enums;

namespace SecureEDM.Client.Contracts.DTOs.Signature;

public class RevokeSignature
{
    public int Id { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public EnumRevokedReason RevokedReason { get; set; }

}