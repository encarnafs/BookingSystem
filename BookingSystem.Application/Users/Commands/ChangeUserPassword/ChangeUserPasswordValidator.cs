using FluentValidation;

namespace BookingSystem.Application.Users.Commands.ChangeUserPassword
{
    /// <summary>
    /// Validador para el comando de cambio de contraseña.
    /// </summary>
    public class ChangeUserPasswordValidator : AbstractValidator<ChangeUserPasswordCommand>
    {
        public ChangeUserPasswordValidator()
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("La nueva contraseña es obligatoria.")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");
        }
    }
}
