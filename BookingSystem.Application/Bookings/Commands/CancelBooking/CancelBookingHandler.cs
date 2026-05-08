using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Application.Bookings.Events;
using MediatR;

namespace BookingSystem.Application.Bookings.Commands.CancelBooking;

public class CancelBookingHandler : IRequestHandler<CancelBookingCommand>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public CancelBookingHandler(
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener la reserva
        var booking = await _bookingRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Booking", request.Id);

        // 2. Cancelar la reserva (regla de dominio)
        booking.Cancel();

        // 3. Actualizar
        await _bookingRepository.UpdateAsync(booking, cancellationToken);

        // 4. Guardar cambios
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Publicar Application Event
        await _mediator.Publish(new BookingCancelledNotification(booking.Id), cancellationToken);
    }
}

