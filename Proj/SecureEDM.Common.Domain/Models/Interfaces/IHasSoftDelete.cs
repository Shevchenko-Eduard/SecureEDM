namespace SecureEDM.Common.Domain.Models.Interfaces;

public interface IHasSoftDelete
{
    public DateTimeOffset? DeleteAt { get; }
}
