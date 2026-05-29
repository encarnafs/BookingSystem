namespace BookingSystem.Domain.Exceptions;

public class InvalidUserStateException : DomainException
{
    public InvalidUserStateException(string state)
        : base($"El estado de usuario '{state}' no es válido.")
    {
    }
}