using MediatR;

namespace BookingSystem.Application.Bookings.Commands.CancelBooking;

public record CancelBookingCommand(Guid Id) : IRequest;
