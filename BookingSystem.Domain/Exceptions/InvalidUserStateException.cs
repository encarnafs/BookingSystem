namespace BookingSystem.Domain.Exceptions;

public class InvalidUserStateException : DomainException
{
    public InvalidUserStateException(string message) : base(message) { }
}