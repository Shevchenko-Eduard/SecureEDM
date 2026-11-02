namespace SecureEDM.Common.Domain.Models.ValueObject;

public class Range<T> : IEquatable<Range<T>> where T : struct, IComparable<T>
{
    public T? From
    {
        get; set
        {
            if (To.HasValue && value.HasValue &&
                To.Value.CompareTo(value.Value) < 0)
            {
                throw ExceptionInvalidDate.DateFromTheFuture;
            }
            field = value;
        }
    }
    public T? To
    {
        get; set
        {
            if (From.HasValue && value.HasValue &&
                From.Value.CompareTo(value.Value) > 0)
            {
                throw ExceptionInvalidDate.DateFromThePast;
            }
            field = value;
        }
    }
    public Range(T? from = null, T? to = null)
    {
        From = from;
        To = to;
    }

    public int Bias(T other)
    {
        if (From.HasValue)
        {
            int from = other.CompareTo(From.Value);
            if (from < 0)
            {
                return from;
            }
        }
        if (To.HasValue)
        {
            int to = other.CompareTo(To.Value);
            if (to > 0)
            {
                return to;
            }
        }
        return 0;
    }

    public bool Contains(T? other)
    {
        if (!other.HasValue) return false;
        if (From.HasValue && other.Value.CompareTo(From.Value) < 0) return false;
        if (To.HasValue && other.Value.CompareTo(To.Value) > 0) return false;
        return true;
    }

    public bool Equals(Range<T>? other)
    {
        if (other == null) return false;
        if (other.From.HasValue != From.HasValue) return false;
        else if (other.From.HasValue && From.HasValue && !other.From.Value.Equals(From.Value)) return false;
        if (other.To.HasValue != To.HasValue) return false;
        else if (other.To.HasValue && To.HasValue && !other.To.Value.Equals(To.Value)) return false;
        return true;
    }
    public override bool Equals(object? obj) => Equals(obj as Range<T>);
    public override int GetHashCode() => HashCode.Combine(From, To);

    public static bool operator ==(Range<T>? left, Range<T>? right) => Equals(left, right);
    public static bool operator !=(Range<T>? left, Range<T>? right) => !Equals(left, right);
}