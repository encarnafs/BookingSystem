using FluentValidation;

namespace BookingSystem.Application.Clients.Commands.DisableClient;

public class DisableClientValidator : AbstractValidator<DisableClientCommand>
{
    public DisableClientValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("El ID del cliente es obligatorio.");
    }
}
