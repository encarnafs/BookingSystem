using BookingSystem.Application.Bookings.Dtos;
using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using MediatR;

namespace BookingSystem.Application.Bookings.Queries.GetBookingsByRoomId;

public class GetBookingsByRoomIdHandler
    : IRequestHandler<GetBookingsByRoomIdQuery, List<BookingDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;

    public GetBookingsByRoomIdHandler(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
    }

    public async Task<List<BookingDto>> Handle(
        GetBookingsByRoomIdQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Validar que la sala existe
        var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (room is null)
            throw new NotFoundException("Room", request.RoomId);

        // 2. Obtener reservas de la sala
        var bookings = await _bookingRepository.GetByRoomAsync(request.RoomId, cancellationToken);

        // 3. Mapear a DTO
        return bookings
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
    }
}
