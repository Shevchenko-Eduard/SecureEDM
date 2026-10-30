using SecureEDM.Common.Domain.Interfaces;

namespace SecureEDM.Common.Infrastructure;

public class Clock : IClock
{
    public DateTimeOffset Now => DateTimeOffset.Now;

    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}