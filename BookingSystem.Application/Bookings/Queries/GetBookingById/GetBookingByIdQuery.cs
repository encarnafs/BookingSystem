using BookingSystem.Application.Bookings.Dtos;
using MediatR;

public record GetBookingByIdQuery(Guid Id) : IRequest<BookingDto>;
