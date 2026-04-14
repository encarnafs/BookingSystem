using FluentValidation;

namespace BookingSystem.Application.Bookings.Queries.GetBookingsByRoomId;

public class GetBookingsByRoomIdValidator
    : AbstractValidator<GetBookingsByRoomIdQuery>
{
    public GetBookingsByRoomIdValidator()
    {
        RuleFor(x => x.RoomId)
            .NotEmpty().WithMessage("El RoomId es obligatorio.");
    }
}
