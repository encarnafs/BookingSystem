using FluentValidation;

namespace BookingSystem.Application.Rooms.Commands.UpdateRoom;

public class UpdateRoomValidator : AbstractValidator<UpdateRoomCommand>
{
    public UpdateRoomValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El Id de la sala es obligatorio.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre de la sala es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("La capacidad debe ser mayor que 0.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es obligatoria.")
            .MaximumLength(500).WithMessage("La descripción no puede superar los 500 caracteres.");
    }
}
