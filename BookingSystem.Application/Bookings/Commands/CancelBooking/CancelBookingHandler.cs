using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using MediatR;

namespace BookingSystem.Application.Bookings.Commands.CancelBooking;

public class CancelBookingHandler : IRequestHandler<CancelBookingCommand>
{
    private readonly IBookingRepository _bookingRepository;

    public CancelBookingHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener la reserva
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId);
        if (booking is null)
            throw new NotFoundException("Booking", request.BookingId);

        // 2. Cancelar la reserva (regla de dominio)
        booking.Cancel();

        // 3. Guardar cambios
        await _bookingRepository.UpdateAsync(booking);
    }
}
