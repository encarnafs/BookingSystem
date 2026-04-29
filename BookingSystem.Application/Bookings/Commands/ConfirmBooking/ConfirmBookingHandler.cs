using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Enums;
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
        // 1. Obtener la reserva por su ID
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking is null)
            throw new NotFoundException("Booking", request.BookingId);

        // 2. Confirmar la reserva (la entidad valida estados)
        booking.Confirm();

        // 3. Guardar los cambios en el repositorio
        await _bookingRepository.UpdateAsync(booking, cancellationToken);
    }

}
