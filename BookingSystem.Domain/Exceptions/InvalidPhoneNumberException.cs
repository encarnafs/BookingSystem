namespace BookingSystem.Domain.Exceptions;

public class InvalidPhoneNumberException : DomainException
{
    public InvalidPhoneNumberException(string message) : base(message) { }
}