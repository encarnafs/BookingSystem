using MediatR;

namespace BookingSystem.Application.Bookings.Commands.ConfirmBooking;

public record ConfirmBookingCommand(Guid Id) : IRequest;
