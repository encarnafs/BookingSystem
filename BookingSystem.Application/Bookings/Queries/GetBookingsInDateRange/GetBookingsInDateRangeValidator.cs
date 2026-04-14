using FluentValidation;

namespace BookingSystem.Application.Bookings.Queries.GetBookingsInDateRange;

public class GetBookingsInDateRangeValidator
    : AbstractValidator<GetBookingsInDateRangeQuery>
{
    public GetBookingsInDateRangeValidator()
    {
        RuleFor(x => x.Start)
            .NotEmpty().WithMessage("La fecha de inicio es obligatoria.");

        RuleFor(x => x.End)
            .NotEmpty().WithMessage("La fecha de fin es obligatoria.");

        RuleFor(x => x)
            .Must(x => x.Start < x.End)
            .WithMessage("La fecha de inicio debe ser anterior a la fecha de fin.");
    }
}
