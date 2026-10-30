using SecureEDM.Common.Domain.Interfaces;

namespace SecureEDM.Common.Domain.Models.Interfaces;

public interface IHasRevocable<TReason> where TReason : Enum
{
    public DateTimeOffset? RevokedAt { get; }
    public TReason? RevokedReason { get; }
    public void Revoke(TReason reason);
}