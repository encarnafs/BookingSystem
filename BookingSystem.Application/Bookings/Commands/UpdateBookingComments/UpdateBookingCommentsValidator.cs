using FluentValidation;

namespace BookingSystem.Application.Bookings.Commands.UpdateBookingComments;

public class UpdateBookingCommentsValidator : AbstractValidator<UpdateBookingCommentsCommand>
{
    public UpdateBookingCommentsValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El BookingId es obligatorio.");

        RuleFor(x => x.Comments)
            .MaximumLength(500).WithMessage("Los comentarios no pueden superar los 500 caracteres.");
    }
}
