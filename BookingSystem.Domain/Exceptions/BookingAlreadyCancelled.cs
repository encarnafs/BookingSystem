using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Domain.Exceptions;
public class BookingAlreadyCancelledException : DomainException
{
    public BookingAlreadyCancelledException(Guid bookingId)
        : base($"La reserva {bookingId} ya está cancelada.")
    {
    }
}
