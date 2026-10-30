namespace SecureEDM.Common.Domain.Interfaces;

public interface IClock
{
    public DateTimeOffset Now { get; }
    public DateTimeOffset UtcNow { get; }
}