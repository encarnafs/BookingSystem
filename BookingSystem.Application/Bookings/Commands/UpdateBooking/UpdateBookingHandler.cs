using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Application.Bookings.Events;
using BookingSystem.Domain.ValueObjects;
using BookingSystem.Domain.Exceptions;
using MediatR;

namespace BookingSystem.Application.Bookings.Commands.UpdateBooking;

public class UpdateBookingHandler : IRequestHandler<UpdateBookingCommand, Unit>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public UpdateBookingHandler(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository,
        IClientRepository clientRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<Unit> Handle(UpdateBookingCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener la reserva
        var booking = await _bookingRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Booking", request.Id);

        // 2. Validar Room
        _ = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken)
            ?? throw new NotFoundException("Room", request.RoomId);

        // 3. Validar Client
        _ = await _clientRepository.GetByIdAsync(request.ClientId, cancellationToken)
            ?? throw new NotFoundException("Client", request.ClientId);

        // 4. Crear DateRange
        var newDateRange = new DateRange(request.Start, request.End);

        // 5. Validar solapamientos
        var hasOverlap = await _bookingRepository.ExistsOverlappingBookingAsync(
            request.RoomId,
            newDateRange.Start,
            newDateRange.End,
            request.Id,
            cancellationToken
        );

        if (hasOverlap)
            throw new BookingOverlapException("La habitación ya está reservada en ese rango de fechas.");

        // 6. Aplicar cambios
        booking.Update(
            request.RoomId,
            request.ClientId,
            newDateRange,
            request.Comments
        );

        // 7. Actualizar
        await _bookingRepository.UpdateAsync(booking, cancellationToken);

        // 8. Guardar cambios
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 9. Publicar Application Event
        await _mediator.Publish(new BookingUpdatedNotification(booking.Id), cancellationToken);

        return Unit.Value;
    }
}


