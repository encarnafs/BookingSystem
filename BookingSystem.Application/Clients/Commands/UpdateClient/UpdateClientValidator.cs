using FluentValidation;

namespace BookingSystem.Application.Clients.Commands.UpdateClient;

public class UpdateClientValidator : AbstractValidator<UpdateClientCommand>
{
    public UpdateClientValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID del cliente es obligatorio.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("El nombre completo es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.")
            .MinimumLength(2).WithMessage("El nombre es demasiado corto.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no es válido.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("El teléfono es obligatorio.")
            .MaximumLength(20).WithMessage("El teléfono no puede superar los 20 caracteres.")
            .Matches(@"^[0-9+\-\s]+$").WithMessage("El teléfono contiene caracteres inválidos."); ;
    }
}
