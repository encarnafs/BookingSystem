namespace BookingSystem.Domain.ValueObjects;

public sealed class DateRange
{
    public DateTime Start { get; }
    public DateTime End { get; }

    private DateRange() { }

    public DateRange(DateTime start, DateTime end)
    {
        if (end <= start)
            throw new ArgumentException("End date debe ser posterior a Start date");

        Start = start;
        End = end;
    }

    public bool Overlaps(DateRange other)
    {
        return Start < other.End && other.Start < End;
    }
}