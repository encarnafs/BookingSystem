namespace BookingSystem.Domain.Exceptions;

public class InvalidRoomStateException : DomainException
{
    public InvalidRoomStateException(string message) : base(message) { }
}