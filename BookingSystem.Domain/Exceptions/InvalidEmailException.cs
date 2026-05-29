namespace BookingSystem.Domain.Exceptions;

public class InvalidEmailException : DomainException
{
    public InvalidEmailException(string email)
        : base($"El email '{email}' no es válido.")
    {
    }
}