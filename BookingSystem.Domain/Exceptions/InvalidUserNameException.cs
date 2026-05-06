namespace BookingSystem.Domain.Exceptions;

public class InvalidUserNameException : DomainException
{
    public InvalidUserNameException(string message) : base(message) { }
}