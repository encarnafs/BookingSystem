namespace BookingSystem.Domain.Exceptions;

public class InvalidUserNameException : DomainException
{
    public InvalidUserNameException(string name)
        : base($"El nombre de usuario '{name}' no es válido.")
    {
    }
}