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

        // 2. Guardar valores anteriores para el evento de auditoría
        var oldValues = new
        {
            Status = booking.Status
        };

        // 3. Cancelar la reserva (regla de dominio)
        booking.Cancel();

        // 4. Guardar cambios
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Publicar Application Event
        await _mediator.Publish(new BookingCancelledNotification(booking.Id, booking.Client.Email.ToString()), cancellationToken);

        // 6. Evento de auditoría
        await _mediator.Publish(
            new BookingUpdatedNotification(                                   
                booking.Id,
                oldValues,
                new { Status = booking.Status }),
            cancellationToken);
    }
}

