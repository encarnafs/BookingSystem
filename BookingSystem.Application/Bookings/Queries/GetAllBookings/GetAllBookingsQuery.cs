using BookingSystem.Application.Bookings.Dtos;
using MediatR;

namespace BookingSystem.Application.Bookings.Queries.GetAllBookings;

public record GetAllBookingsQuery() : IRequest<IReadOnlyList<BookingDto>>;
