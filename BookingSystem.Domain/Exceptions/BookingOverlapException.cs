using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Domain.Exceptions;

public class BookingOverlapException : Exception
{
    public BookingOverlapException(string message) : base(message)
    {
    }
}
