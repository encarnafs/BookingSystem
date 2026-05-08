using FluentValidation;

namespace BookingSystem.Application.Rooms.Commands.CreateRoom;

public class CreateRoomValidator : AbstractValidator<CreateRoomCommand>
{
    public CreateRoomValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre de la sala es obligatorio")
            .MaximumLength(100).WithMessage("El nombre de la sala no puede exceder los 100 caracteres");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("La capacidad debe ser mayor que cero");
    }
}
