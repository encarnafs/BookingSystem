using BookingSystem.Application.Bookings.Commands.CreateBooking;
using BookingSystem.Application.Bookings.Dtos;
using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.ValueObjects;
using MediatR;

namespace BookingSystem.Application.Bookings.Commands.CreateBooking;
public class CreateBookingHandler : IRequestHandler<CreateBookingCommand, BookingDto>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;

    public CreateBookingHandler(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository,
        IClientRepository clientRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUser)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _clientRepository = clientRepository;
        _userRepository = userRepository;
        _currentUser = currentUser;
    }

    public async Task<BookingDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar Room
        var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken)
            ?? throw new NotFoundException("Room", request.RoomId);

        // 2. Validar Client
        var client = await _clientRepository.GetByIdAsync(request.ClientId, cancellationToken)
            ?? throw new NotFoundException("Client", request.ClientId);

        // 3. Obtener el usuario creador (si viene del token)
        var createdByUserId = _currentUser.UserId ?? Guid.Empty;

        if (createdByUserId != Guid.Empty)
        {
            // Esto valida que el usuario existe sin crear una variable innecesaria.
            _ = await _userRepository.GetByIdAsync(createdByUserId, cancellationToken)
                ?? throw new NotFoundException("User", createdByUserId);

        }

        // 4. Crear DateRange (el VO valida fechas inválidas)
        var dateRange = new DateRange(request.Start, request.End);

        // 5. Validar solapamientos
        var overlaps = await _bookingRepository.ExistsOverlappingBookingAsync(
            request.RoomId,
            request.Start,
            request.End,
            null, // No excluir ninguna reserva (es creación)
            cancellationToken
        );

        if (overlaps)
            throw new ValidationException("La sala ya está reservada para las fechas seleccionadas");

        // 6. Crear la reserva usando el dominio
        var booking = new Booking(
            room.Id,
            client.Id,
            createdByUserId,
            dateRange,
            request.Comments
        );

        // 7. Guardar
        await _bookingRepository.AddAsync(booking, cancellationToken);

        // 8. Devolver DTO
        return new BookingDto
        {
            Id = booking.Id,
            RoomId = booking.RoomId,
            ClientId = booking.ClientId,
            CreatedByUserId = booking.CreatedByUserId,
            Start = booking.DateRange.Start,
            End = booking.DateRange.End,
            Comments = booking.Comments,
            Status = booking.Status.ToString(),
            CreatedAt = booking.CreatedAt
        };
    }

}

