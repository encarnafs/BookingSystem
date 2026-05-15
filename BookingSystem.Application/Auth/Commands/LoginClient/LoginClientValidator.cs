using FluentValidation;

namespace BookingSystem.Application.Auth.Commands.LoginClient;

public class LoginClientValidator : AbstractValidator<LoginClientCommand>
{
    public LoginClientValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}