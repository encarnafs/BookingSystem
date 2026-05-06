using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Domain.Exceptions;
public class OverlappingBookingException : DomainException
{
    public OverlappingBookingException(Guid roomId)
        : base($"La sala {roomId} ya está reservada en ese rango de fechas.")
    {
    }
}
