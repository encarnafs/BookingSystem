using FluentValidation;

namespace BookingSystem.Application.Bookings.Commands.CancelBooking;

public class CancelBookingValidator : AbstractValidator<CancelBookingCommand>
{
    public CancelBookingValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El BookingId es obligatorio.");
    }
}
