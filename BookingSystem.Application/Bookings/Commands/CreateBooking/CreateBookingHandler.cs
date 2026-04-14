using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.ValueObjects;
using MediatR;

namespace BookingSystem.Application.Bookings.Commands.CreateBooking;

public class CreateBookingHandler : IRequestHandler<CreateBookingCommand, Guid>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IUserRepository _userRepository;

    public CreateBookingHandler(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository,
        IClientRepository clientRepository,
        IUserRepository userRepository)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _clientRepository = clientRepository;
        _userRepository = userRepository;
    }

    public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar que la sala existe
        var room = await _roomRepository.GetByIdAsync(request.RoomId);
        if (room is null)
            throw new NotFoundException("Room", request.RoomId);

        // 2. Validar que el cliente existe
        var client = await _clientRepository.GetByIdAsync(request.ClientId);
        if (client is null)
            throw new NotFoundException("Client", request.ClientId);

        // 3. Validar que el usuario existe
        var user = await _userRepository.GetByIdAsync(request.CreatedByUserId);
        if (user is null)
            throw new NotFoundException("User", request.CreatedByUserId);

        // 4. Validar fechas
        if (request.Start >= request.End)
            throw new ValidationException("Fecha Start debe ser antes de Fecha End");

        var dateRange = new DateRange(request.Start, request.End);

        // 5. Validar solapamientos
        bool overlaps = await _bookingRepository.ExistsOverlappingBookingAsync(
            request.RoomId,
            request.Start,
            request.End
        );

        if (overlaps)
            throw new ValidationException("La sala ya está reservada para las fechas seleccionadas");

        // 6. Crear la reserva
        var booking = new Booking(
            room.Id,
            client.Id,
            user.Id,
            dateRange,
            request.Comments
        );

        // 7. Guardar en base de datos
        await _bookingRepository.AddAsync(booking);

        return booking.Id;
    }
}

