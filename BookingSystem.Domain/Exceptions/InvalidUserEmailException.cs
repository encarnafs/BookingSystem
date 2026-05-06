namespace BookingSystem.Domain.Exceptions;

public class InvalidUserEmailException : DomainException
{
    public InvalidUserEmailException(string message) : base(message) { }
}