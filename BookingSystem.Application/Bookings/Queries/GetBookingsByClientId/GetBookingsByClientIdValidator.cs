using BookingSystem.Application.Bookings.Queries.GetBookingsByClientId;
using FluentValidation;

public class GetBookingsByClientIdValidator : AbstractValidator<GetBookingsByClientIdQuery>
{
    public GetBookingsByClientIdValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("El ClientId es obligatorio.");
    }
}
