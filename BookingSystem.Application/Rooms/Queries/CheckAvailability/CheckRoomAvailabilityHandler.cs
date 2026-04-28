using BookingSystem.Application.Bookings.Dtos;
using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Application.Rooms.Dtos;
using MediatR;

namespace BookingSystem.Application.Rooms.Queries.CheckAvailability;

public class CheckRoomAvailabilityHandler
    : IRequestHandler<CheckRoomAvailabilityQuery, RoomAvailabilityDto>
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;

    public CheckRoomAvailabilityHandler(
        IRoomRepository roomRepository,
        IBookingRepository bookingRepository)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<RoomAvailabilityDto> Handle(
        CheckRoomAvailabilityQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Validar que la sala existe
        var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (room is null)
            throw new NotFoundException("Room", request.RoomId);

        // 2. Obtener reservas de la sala
        var bookings = await _bookingRepository.GetByRoomAsync(request.RoomId, cancellationToken);

        // 3. Buscar solapamientos
        var conflicts = bookings
            .Where(b => request.Start < b.DateRange.End && request.End > b.DateRange.Start)
            .Select(b => new BookingDto
            {
                Id = b.Id,
                RoomId = b.RoomId,
                ClientId = b.ClientId,
                CreatedByUserId = b.CreatedByUserId,
                Start = b.DateRange.Start,
                End = b.DateRange.End,
                Comments = b.Comments,
                Status = b.Status.ToString(),
                CreatedAt = b.CreatedAt,
                RoomName = b.Room.Name,
                ClientFullName = b.Client.FullName
            })
            .ToList();

        return new RoomAvailabilityDto
        {
            IsAvailable = !conflicts.Any(),
            ConflictingBookings = conflicts
        };
    }
}
