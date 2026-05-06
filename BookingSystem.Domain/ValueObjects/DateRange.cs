using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Domain.ValueObjects;

public class DateRange
{
    public DateTime Start { get; }
    public DateTime End { get; }

    public DateRange(DateTime start, DateTime end)
    {
        if (start == default || end == default)
            throw new InvalidDateRangeException(start, end);

        if (end <= start)
            throw new InvalidDateRangeException(start, end);

        Start = start;
        End = end;
    }

    public bool Overlaps(DateRange other)
    {
        if (other is null)
            throw new InvalidDateRangeException(DateTime.MinValue, DateTime.MinValue);

        // Dos rangos se solapan si:
        // Start < other.End && End > other.Start
        return Start < other.End && other.Start < End;
    }

    public override string ToString()
        => $"{Start:yyyy-MM-dd HH:mm} → {End:yyyy-MM-dd HH:mm}";
}
