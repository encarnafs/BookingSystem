namespace BookingSystem.Domain.Exceptions;

public class InvalidUserRoleException : DomainException
{
    public InvalidUserRoleException(string message) : base(message) { }
}