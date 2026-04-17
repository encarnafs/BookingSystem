using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using MediatR;

namespace BookingSystem.Application.Bookings.Commands.ConfirmBooking;

public class ConfirmBookingHandler : IRequestHandler<ConfirmBookingCommand>
{
    private readonly IBookingRepository _bookingRepository;

    public ConfirmBookingHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener la reserva
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking is null)
            throw new NotFoundException("Booking", request.BookingId);

        // 2. Confirmar la reserva (regla de dominio)
        booking.Confirm();

        // 3. Guardar cambios
        await _bookingRepository.UpdateAsync(booking, cancellationToken);
    }
}
