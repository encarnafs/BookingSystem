namespace BookingSystem.Domain.Exceptions;

public class InvalidUserEmailException : DomainException
{
    public InvalidUserEmailException(string email)
        : base($"El email de usuario '{email}' no es válido.")
    {
    }
}