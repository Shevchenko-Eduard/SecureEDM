using SecureEDM.Common.Domain.Enums;

namespace SecureEDM.Client.Contracts.DTOs.AsymmetricKey;

public class RevokeAsymmetricKey
{
    public int Id { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public EnumRevokedReason RevokedReason { get; set; }

}