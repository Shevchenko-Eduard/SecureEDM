namespace SecureEDM.Common.Domain.Exceptions;

public class ExceptionAttested(string? message = null, Exception? innerException = null) : Exception(message, innerException)
{
    public static ExceptionAttested AttestedAlreadyExist { get; } = new("Attested already exists.");
}