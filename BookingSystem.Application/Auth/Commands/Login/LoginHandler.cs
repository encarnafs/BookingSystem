using BookingSystem.Application.Auth.Commands.Login;
using BookingSystem.Application.Common.Interfaces;
using MediatR;

namespace BookingSystem.Application.Auth.Commands.Login;

public class LoginHandler : IRequestHandler<LoginCommand, string>
{
    private readonly IAuthService _authService;
    private readonly IJwtTokenGenerator _jwt;

    public LoginHandler(IAuthService authService, IJwtTokenGenerator jwt)
    {
        _authService = authService;
        _jwt = jwt;
    }

    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _authService.ValidateUserAsync(
            request.Email,
            request.Password,
            cancellationToken);

        //Cambiar el error a devolver con formato Json
        if (user is null)
            throw new Exception("Credenciales inválidas.");

        return _jwt.GenerateToken(
            user.Id,
            user.Email,
            user.Username,
            user.Role);
    }
}
