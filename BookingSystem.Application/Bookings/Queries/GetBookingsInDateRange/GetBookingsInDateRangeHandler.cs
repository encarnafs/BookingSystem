using BookingSystem.Application.Bookings.Dtos;
using BookingSystem.Application.Common.Interfaces;
using MediatR;

namespace BookingSystem.Application.Bookings.Queries.GetBookingsInDateRange;

public class GetBookingsInDateRangeHandler
    : IRequestHandler<GetBookingsInDateRangeQuery, List<BookingDto>>
{
    private readonly IBookingRepository _bookingRepository;

    public GetBookingsInDateRangeHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<List<BookingDto>> Handle(
        GetBookingsInDateRangeQuery request,
        CancellationToken cancellationToken)
    {
        var bookings = await _bookingRepository.GetInDateRangeAsync(
            request.Start,
            request.End,
            cancellationToken
        );

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
                CreatedAt = b.CreatedAt
            })
            .ToList();
    }
}
