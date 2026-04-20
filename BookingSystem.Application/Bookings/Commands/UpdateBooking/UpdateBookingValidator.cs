using BookingSystem.Domain.ValueObjects;
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

        RuleFor(x => x.DateRange)
            .NotNull().WithMessage("El rango de fechas es obligatorio");

        RuleFor(x => x.DateRange.Start)
            .LessThan(x => x.DateRange.End)
            .WithMessage("La fecha de inicio debe ser anterior a la fecha de fin");
    }
}
