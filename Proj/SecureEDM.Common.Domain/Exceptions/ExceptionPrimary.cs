namespace SecureEDM.Common.Domain.Exceptions;

public class ExceptionPrimary(string? message = null, Exception? innerException = null) : Exception(message, innerException)
{
    public static ExceptionPrimary AlreadySetPrimary { get; } = new("Cannot be set primary again.");
}