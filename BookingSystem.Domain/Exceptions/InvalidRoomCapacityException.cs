namespace BookingSystem.Domain.Exceptions;

public class InvalidRoomCapacityException : DomainException
{
    public InvalidRoomCapacityException(string message) : base(message) { }
}