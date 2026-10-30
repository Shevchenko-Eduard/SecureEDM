using SecureEDM.Common.Domain.Interfaces;

public class ExceptionInvalidDate(string? message = null, Exception? innerException = null) : Exception(message, innerException)
{
    public static ExceptionInvalidDate DateFromThePast { get; } = new("Date from the past.");
    public static ExceptionInvalidDate DateFromTheFuture { get; } = new("Date from the future.");
    public static ExceptionInvalidDate DateIsNotNow { get; } = new("Date is not now.");
    public static void CallDateFromThePast(IClock clock, DateTimeOffset date, TimeSpan infelicity = default)
    {
        if (date + infelicity > clock.Now) return;
        throw DateFromThePast;
    }
    public static void CallDateFromTheFuture(IClock clock, DateTimeOffset date, TimeSpan infelicity = default)
    {
        if (date - infelicity < clock.Now) return;
        throw DateFromTheFuture;
    }
    public static void GetDateIsNotNow(IClock clock, DateTimeOffset date, TimeSpan infelicity = default)
    {
        try
        {
            CallDateFromThePast(clock, date, infelicity);
            CallDateFromTheFuture(clock, date, infelicity);
        }
        catch (ExceptionInvalidDate ex)
        {
            throw new ExceptionInvalidDate(DateIsNotNow.Message, ex);
        }
    }
}