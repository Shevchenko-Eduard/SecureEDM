using System.Numerics;

public class ExceptionInvalidId(string? message = null, Exception? innerException = null) : Exception(message, innerException)
{
    public static ExceptionInvalidId IdIsLessThanOrEqualToZero { get; } = new("ID is less than or equal to zero.");
    public static void CallIdIsLessThanOrEqualToZero<TId>(TId id) where TId : INumber<TId>
    {
        if (id.CompareTo(0) > 0) return;
        throw IdIsLessThanOrEqualToZero;
    }
}