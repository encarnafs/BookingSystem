namespace BookingSystem.Application.Bookings.Commands.UpdateBooking;

using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.ValueObjects;
using MediatR;

public  class UpdateBookingHandler : IRequestHandler<UpdateBookingCommand, Unit>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IClientRepository _clientRepository;

    public UpdateBookingHandler(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository,
        IClientRepository clientRepository)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _clientRepository = clientRepository;
    }

    public async Task<Unit> Handle(UpdateBookingCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener la reserva
        var booking = await _bookingRepository.GetByIdAsync(request.Id, cancellationToken);
        if (booking is null)
            throw new KeyNotFoundException($"No existe la reserva con Id {request.Id}");

        // 2. Validar Room
        var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (room is null)
            throw new KeyNotFoundException($"No existe la habitación con Id {request.RoomId}");

        // 3. Validar Client
        var client = await _clientRepository.GetByIdAsync(request.ClientId, cancellationToken);
        if (client is null)
            throw new KeyNotFoundException($"No existe el cliente con Id {request.ClientId}");

        // 4. Validar solapamientos
        var hasOverlap = await _bookingRepository.ExistsOverlappingBookingAsync(
            request.RoomId,
            request.DateRange.Start,
            request.DateRange.End,
            cancellationToken);

        if (hasOverlap)
            throw new InvalidOperationException("La habitación ya está reservada en ese rango de fechas.");

        // 5. Aplicar cambios usando el dominio
        booking.Update(
            request.RoomId,
            request.ClientId,
            request.DateRange,
            request.Comments
        );

        // 6. Guardar cambios
        await _bookingRepository.UpdateAsync(booking, cancellationToken);

        return Unit.Value;
    }
}

