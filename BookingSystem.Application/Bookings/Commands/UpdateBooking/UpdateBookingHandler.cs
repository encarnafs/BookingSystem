using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.ValueObjects;
using MediatR;

namespace BookingSystem.Application.Bookings.Commands.UpdateBooking;

public class UpdateBookingHandler : IRequestHandler<UpdateBookingCommand, Unit>
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
        var booking = await _bookingRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"No existe la reserva con Id {request.Id}");

        // 2. Validar Room
        _ = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken)
            ?? throw new KeyNotFoundException($"No existe la habitación con Id {request.RoomId}");

        // 3. Validar Client
        _ = await _clientRepository.GetByIdAsync(request.ClientId, cancellationToken)
            ?? throw new KeyNotFoundException($"No existe el cliente con Id {request.ClientId}");

        // 4. Crear DateRange (el VO valida fechas inválidas)
        var newDateRange = new DateRange(request.Start, request.End);

        // 5. Validar solapamientos excluyendo la propia reserva
        var hasOverlap = await _bookingRepository.ExistsOverlappingBookingAsync(
            request.RoomId,
            request.Start,
            request.End,
            request.Id, // excluir la propia reserva
            cancellationToken
        );

        if (hasOverlap)
            throw new InvalidOperationException("La habitación ya está reservada en ese rango de fechas.");

        // 6. Aplicar cambios usando el dominio
        booking.Update(
            request.RoomId,
            request.ClientId,
            newDateRange,
            request.Comments
        );

        // 7. Guardar cambios
        await _bookingRepository.UpdateAsync(booking, cancellationToken);

        return Unit.Value;
    }
}


