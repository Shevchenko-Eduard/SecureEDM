namespace SecureEDM.Common.Domain.Models.Interfaces;

public interface IHasSoftDelete
{
    public DateTimeOffset? DeleteAt { get; }
    public void SoftDelete();
    public void SoftDelete(DateTimeOffset deleteAt);
}
