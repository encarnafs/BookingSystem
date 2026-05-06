using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Domain.Exceptions;
public class BookingAlreadyConfirmedException : DomainException
{
    public BookingAlreadyConfirmedException(Guid bookingId)
        : base($"La reserva {bookingId} ya está confirmada.")
    {
    }
}
