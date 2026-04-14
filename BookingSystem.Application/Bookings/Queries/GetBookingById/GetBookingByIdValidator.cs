using FluentValidation;

public class GetBookingByIdValidator : AbstractValidator<GetBookingByIdQuery>
{
    public GetBookingByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El Id de la reserva es obligatorio.");
    }
}
