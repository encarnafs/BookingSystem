using FluentValidation;

namespace BookingSystem.Application.Clients.Commands.CreateClient;

public class CreateClientValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("El nombre completo es obligatorio");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio")
            .EmailAddress().WithMessage("El email no es válido");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("El teléfono es obligatorio");
    }
}
