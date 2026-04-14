using FluentValidation;

namespace BookingSystem.Application.Bookings.Commands.ConfirmBooking;

public class ConfirmBookingValidator : AbstractValidator<ConfirmBookingCommand>
{
    public ConfirmBookingValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("El BookingId es obligatorio.");
    }
}
