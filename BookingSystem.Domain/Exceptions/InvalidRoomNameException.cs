namespace BookingSystem.Domain.Exceptions;

public class InvalidRoomNameException : DomainException
{
    public InvalidRoomNameException(string name)
        : base($"El nombre de sala '{name}' no es válido.")
    {
    }
}