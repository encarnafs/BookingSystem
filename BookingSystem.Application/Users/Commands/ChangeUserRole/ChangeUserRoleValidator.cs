using FluentValidation;

namespace BookingSystem.Application.Users.Commands.ChangeUserRole
{
    /// <summary>
    /// Validador para el comando de cambio de rol.
    /// </summary>
    public class ChangeUserRoleValidator : AbstractValidator<ChangeUserRoleCommand>
    {
        public ChangeUserRoleValidator()
        {
            RuleFor(x => x.NewRole)
                .NotEmpty().WithMessage("El nuevo rol es obligatorio.")
                .Must(role => role == "Admin" || role == "User")
                .WithMessage("El rol debe ser 'Admin' o 'User'.");
        }
    }
}
