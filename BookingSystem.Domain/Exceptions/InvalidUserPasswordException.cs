
using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Domain.Exceptions;
public class InvalidUserPasswordException : DomainException
{
    public InvalidUserPasswordException(string password)
        : base($"La contraseña proporcionada no es válida.")
    {
    }
}