using BookingSystem.Application.Bookings.Dtos;
using BookingSystem.Application.Common.Interfaces;
using MediatR;

namespace BookingSystem.Application.Bookings.Queries.GetAllBookings;

public class GetAllBookingsHandler : IRequestHandler<GetAllBookingsQuery, IReadOnlyList<BookingDto>>
{
    private readonly IBookingRepository _bookingRepository;

    public GetAllBookingsHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<IReadOnlyList<BookingDto>> Handle(GetAllBookingsQuery request, CancellationToken cancellationToken)
    {
        var bookings = await _bookingRepository.GetAllAsync(cancellationToken);

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
                CreatedAt = b.CreatedAt,
                Status = b.Status.ToString(),
                RoomName = b.Room.Name,
                ClientFullName = b.Client.FullName
            })
            .ToList();
    }
}
