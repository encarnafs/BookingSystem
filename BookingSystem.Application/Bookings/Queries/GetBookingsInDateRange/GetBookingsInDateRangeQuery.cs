using BookingSystem.Application.Bookings.Dtos;
using MediatR;

namespace BookingSystem.Application.Bookings.Queries.GetBookingsInDateRange;

public record GetBookingsInDateRangeQuery(DateTime Start, DateTime End)
    : IRequest<List<BookingDto>>;
