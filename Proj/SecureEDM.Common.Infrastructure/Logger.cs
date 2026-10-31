using SecureEDM.Common.Contracts.Interfaces;
using SecureEDM.Common.Domain.Interfaces;

namespace SecureEDM.Common.Infrastructure;

public class Logger<T>(ILoggerService logger, IClock clock) : ILogger<T>
{
    private readonly ILoggerService _logger = logger;
    private readonly IClock _clock = clock;

    public void Trace(string message) => Log(EnumLoggerLevel.Trace, message);
    public void Debug(string message) => Log(EnumLoggerLevel.Debug, message);
    public void Info(string message) => Log(EnumLoggerLevel.Info, message);
    public void Warning(string message) => Log(EnumLoggerLevel.Warning, message);
    public void Error(Exception ex, string? message = null)
    {
        string fullMessage = string.IsNullOrEmpty(message)
            ? ex.ToString()
            : $"{message}\n{ex}";

        Log(EnumLoggerLevel.Error, fullMessage);
    }

    private void Log(EnumLoggerLevel level, string message)
    {
        string log = $"[{_clock.Now:O}] {level} ({typeof(T).FullName}): {message}";
        _ = _logger.Send(level, log);
    }
}