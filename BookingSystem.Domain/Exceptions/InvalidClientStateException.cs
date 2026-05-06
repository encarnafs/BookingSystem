namespace BookingSystem.Domain.Exceptions;

public class InvalidClientStateException : DomainException
{
    public InvalidClientStateException(string message) : base(message) { }
}