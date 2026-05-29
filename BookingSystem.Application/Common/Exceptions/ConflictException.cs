namespace BookingSystem.Application.Common.Exceptions;

public class ConflictException : Exception
{
    public ConflictException(string value)
    : base($"Conflict: {value}")
    {
    }
}
