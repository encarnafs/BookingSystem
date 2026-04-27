using BookingSystem.Application.Bookings.Dtos;
using BookingSystem.Application.Common.Interfaces;
using MediatR;

namespace BookingSystem.Application.Bookings.Queries.GetBookingById;

public class GetBookingByIdHandler
    : IRequestHandler<GetBookingByIdQuery, BookingDto?>
{
    private readonly IBookingRepository _bookingRepository;

    public GetBookingByIdHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<BookingDto?> Handle(
        GetBookingByIdQuery request,
        CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.Id, cancellationToken);

        if (booking is null)
            return null;

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
            CreatedAt = booking.CreatedAt,
            RoomName = booking.Room.Name,
            ClientFullName = booking.Client.FullName
        };
    }
}
