using BookingSystem.Application.Bookings.Dtos;
using MediatR;

namespace BookingSystem.Application.Bookings.Queries.GetBookingsByClientId;

public record GetBookingsByClientIdQuery(Guid ClientId)
    : IRequest<List<BookingDto>>;
