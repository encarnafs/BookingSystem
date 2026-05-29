namespace BookingSystem.Domain.Exceptions;

public class InvalidRoomStateException : DomainException
{
    public InvalidRoomStateException(string state)
        : base($"El estado de la sala '{state}' no es válido.")
    {
    }
}