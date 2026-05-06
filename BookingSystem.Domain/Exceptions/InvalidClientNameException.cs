namespace BookingSystem.Domain.Exceptions;

public class InvalidClientNameException : DomainException
{
    public InvalidClientNameException(string message) : base(message) { }
}