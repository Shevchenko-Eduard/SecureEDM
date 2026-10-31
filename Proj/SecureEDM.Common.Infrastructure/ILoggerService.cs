namespace SecureEDM.Common.Infrastructure;

public interface ILoggerService
{
    public Task Send(EnumLoggerLevel level, string message);
}