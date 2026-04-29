using FluentValidation;

namespace BookingSystem.Application.Bookings.Commands.CreateBooking;

public class CreateBookingValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingValidator()
    {
        RuleFor(x => x.RoomId).NotEmpty();
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.Start).LessThan(x => x.End)
            .WithMessage("La fecha de inicio debe ser anterior a la fecha de fin");
    }
}
