namespace BookingSystem.Domain.Exceptions;

public class InvalidRoomNameException : DomainException
{
    public InvalidRoomNameException(string message) : base(message) { }
}