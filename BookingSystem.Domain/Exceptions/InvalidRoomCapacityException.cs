namespace BookingSystem.Domain.Exceptions;

public class InvalidRoomCapacityException : DomainException
{
    public InvalidRoomCapacityException(int capacity)
       : base($"La capacidad '{capacity}' no es válida para una sala.")
    {
    }
}