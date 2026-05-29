namespace BookingSystem.Domain.Exceptions;

public class InvalidClientNameException : DomainException
{
    public InvalidClientNameException(string name)
        : base($"El nombre del cliente '{name}' no es válido.")
    {
    }
}