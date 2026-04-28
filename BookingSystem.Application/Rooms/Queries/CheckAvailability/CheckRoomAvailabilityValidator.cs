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
            .WithMessage("Start date must be earlier than End date.");
    }
}
