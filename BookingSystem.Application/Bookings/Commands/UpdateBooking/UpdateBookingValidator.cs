using FluentValidation;

namespace BookingSystem.Application.Bookings.Commands.UpdateBooking;

public class UpdateBookingValidator : AbstractValidator<UpdateBookingCommand>
{
    public UpdateBookingValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El Id no puede estar vacío");

        RuleFor(x => x.RoomId)
            .NotEmpty().WithMessage("El RoomId no puede estar vacío");

        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("El ClientId no puede estar vacío");

        RuleFor(x => x.Start)
            .NotEmpty().WithMessage("La fecha de inicio es obligatoria");

        RuleFor(x => x.End)
            .NotEmpty().WithMessage("La fecha de fin es obligatoria");

        RuleFor(x => x)
            .Must(x => x.Start < x.End)
            .WithMessage("La fecha de inicio debe ser anterior a la fecha de fin");
    }
}
