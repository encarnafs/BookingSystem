using FluentValidation;

namespace BookingSystem.Application.Rooms.Queries.CheckAvailability;

public class CheckRoomAvailabilityValidator : AbstractValidator<CheckRoomAvailabilityQuery>
{
    public CheckRoomAvailabilityValidator()
    {
        RuleFor(x => x.RoomId)
            .NotEmpty();

        RuleFor(x => x.Start)
            .LessThan(x => x.End)
            .WithMessage("La fecha de inicio debe ser anterior a la fecha de fin.");
    }
}
