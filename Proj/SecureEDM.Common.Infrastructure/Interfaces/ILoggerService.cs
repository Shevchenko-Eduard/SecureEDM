using SecureEDM.Common.Infrastructure.Enums;

namespace SecureEDM.Common.Infrastructure.Interfaces;

public interface ILoggerService
{
    public Task Send(EnumLoggerLevel level, string message);
}