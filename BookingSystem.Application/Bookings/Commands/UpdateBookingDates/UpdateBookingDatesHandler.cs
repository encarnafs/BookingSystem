using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Application.Bookings.Events;
using BookingSystem.Domain.Exceptions;
using BookingSystem.Domain.ValueObjects;
using MediatR;

namespace BookingSystem.Application.Bookings.Commands.UpdateBookingDates;

public class UpdateBookingDatesHandler : IRequestHandler<UpdateBookingDatesCommand>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public UpdateBookingDatesHandler(
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task Handle(UpdateBookingDatesCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener la reserva
        var booking = await _bookingRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Booking", request.Id);

        // 2. Guardar los valores antiguos
        var oldValues = new
        {
            Start = booking.DateRange.Start,
            End = booking.DateRange.End
        };

        // 3. Crear el nuevo rango de fechas
        var newDateRange = new DateRange(request.Start, request.End);

        // 4. Validar solapamientos excluyendo la propia reserva
        var hasOverlap = await _bookingRepository.ExistsOverlappingBookingAsync(
            booking.RoomId,
            newDateRange.Start,
            newDateRange.End,
            booking.Id,
            cancellationToken
        );

        if (hasOverlap)
            throw new OverlappingBookingException(booking.RoomId);

        // 5. Actualizar la entidad
        booking.UpdateDates(newDateRange);

        // 6. Guardar cambios
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Publicar Application Event
        await _mediator.Publish(new BookingDatesUpdatedNotification(booking.Id, booking.Client.Email.ToString()), cancellationToken);

        // 8. Auditoría: Publicar evento con valores antiguos y nuevos
        await _mediator.Publish(
            new BookingUpdatedNotification(
                booking.Id,
                oldValues,
                new
                {
                    Start = booking.DateRange.Start,
                    End = booking.DateRange.End
                }),
            cancellationToken);
    }
}
