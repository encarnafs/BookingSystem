using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Domain.Exceptions;
public class InvalidBookingStateException : DomainException
{
    public InvalidBookingStateException(string message)
        : base(message)
    {
    }
}
