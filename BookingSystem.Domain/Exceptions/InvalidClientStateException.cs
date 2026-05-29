namespace BookingSystem.Domain.Exceptions;

public class InvalidClientStateException : DomainException
{
    public InvalidClientStateException(string state)
        : base($"El estado del cliente '{state}' no es válido.")
    {
    }
}