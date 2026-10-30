using System.Collections;
using System.Numerics;

public class ExceptionNullableValue(string? message = null, Exception? innerException = null) : Exception(message, innerException)
{
    public static ExceptionNullableValue ValueIsNull { get; } = new("Value is null.");
    public static void CallValueIsNull(ICollection value)
    {
        if (value is not null && value.Count > 0) return;
        throw ValueIsNull;
    }
    public static void CallValueIsNull<TValue>(TValue value)
    {
        if (value is not null) return;
        throw ValueIsNull;
    }
    public static void CallValueIsNull(string value)
    {
        if (!string.IsNullOrWhiteSpace(value)) return;
        throw ValueIsNull;
    }
}