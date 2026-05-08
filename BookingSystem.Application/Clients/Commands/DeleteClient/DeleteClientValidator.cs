using FluentValidation;

namespace BookingSystem.Application.Clients.Commands.DeleteClient;

public class DeleteClientValidator : AbstractValidator<DeleteClientCommand>
{
    public DeleteClientValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("El ID del cliente es obligatorio.");
    }
}
