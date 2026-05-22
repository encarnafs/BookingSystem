namespace BookingSystem.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message, object key)
    : base(message)
    {
    }
}
