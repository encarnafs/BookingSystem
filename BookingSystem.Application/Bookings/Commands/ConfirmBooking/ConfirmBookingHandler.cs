using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Application.Bookings.Events;
using MediatR;

namespace BookingSystem.Application.Bookings.Commands.ConfirmBooking;

public class ConfirmBookingHandler : IRequestHandler<ConfirmBookingCommand>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public ConfirmBookingHandler(
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener la reserva
        var booking = await _bookingRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Booking", request.Id);

        // 2. Guardar los valores antiguos para la auditoría
        var oldValues = new
        {
            Status = booking.Status
        };

        // 3. Confirmar la reserva (regla de dominio)
        booking.Confirm();

        // 4. Guardar cambios
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Publicar Application Event
        await _mediator.Publish(new BookingConfirmedNotification(booking.Id, booking.Client.Email.ToString()), cancellationToken);

        // 6. Auditoría
        await _mediator.Publish(
        new BookingUpdatedNotification(
            booking.Id,
            oldValues,
            new { Status = booking.Status }),
        cancellationToken);
    }
}

