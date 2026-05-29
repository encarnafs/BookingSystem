namespace BookingSystem.Domain.Exceptions;

public class InvalidUserRoleException : DomainException
{
    public InvalidUserRoleException(string role)
        : base($"El rol '{role}' no es válido.")
    {
    }
}