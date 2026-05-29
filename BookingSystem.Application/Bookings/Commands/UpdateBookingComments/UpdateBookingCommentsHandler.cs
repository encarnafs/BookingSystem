using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Application.Bookings.Events;
using MediatR;

namespace BookingSystem.Application.Bookings.Commands.UpdateBookingComments;

public class UpdateBookingCommentsHandler : IRequestHandler<UpdateBookingCommentsCommand>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public UpdateBookingCommentsHandler(
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task Handle(UpdateBookingCommentsCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener la reserva
        var booking = await _bookingRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Booking", request.Id);

        // 2. Guardar valores antiguos para Auditoría
        var oldValues = new
        {
            booking.Comments
        };

        // 3. Actualizar comentarios (regla de dominio)
        booking.UpdateComments(request.Comments);

        // 4. Guardar cambios
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Publicar Application Event
        await _mediator.Publish(new BookingCommentsUpdatedNotification(booking.Id, booking.Client.Email.ToString()), cancellationToken);

        // 6.Auditoría
        await _mediator.Publish(
        new BookingUpdatedNotification(
            booking.Id,
            oldValues,
            new { booking.Comments }),
        cancellationToken);
    }
}

