using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using MediatR;

namespace BookingSystem.Application.Bookings.Commands.UpdateBookingComments;

public class UpdateBookingCommentsHandler : IRequestHandler<UpdateBookingCommentsCommand>
{
    private readonly IBookingRepository _bookingRepository;

    public UpdateBookingCommentsHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task Handle(UpdateBookingCommentsCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener la reserva
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking is null)
            throw new NotFoundException("Booking", request.BookingId);

        // 2. Actualizar comentarios
        booking.UpdateComments(request.Comments);

        // 3. Guardar cambios
        await _bookingRepository.UpdateAsync(booking, cancellationToken);
    }
}
