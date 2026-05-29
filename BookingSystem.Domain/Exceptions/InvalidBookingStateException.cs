using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Domain.Exceptions;
public class InvalidBookingStateException : DomainException
{
    public InvalidBookingStateException(string state)
        : base($"El estado de la reserva '{state}' no es válido.")
    {
    }
}
