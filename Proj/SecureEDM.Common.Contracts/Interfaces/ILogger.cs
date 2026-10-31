namespace SecureEDM.Common.Contracts.Interfaces;

public interface ILogger<T>
{
    public void Trace(string message);
    public void Debug(string message);
    public void Info(string message);
    public void Warning(string message);
    public void Error(Exception ex, string? message = null);
}
