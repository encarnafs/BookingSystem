using BookingSystem.Application.Auth.Commands.Login;
using BookingSystem.Application.Auth.Responses;
using BookingSystem.Application.Common.Interfaces;
using MediatR;

namespace BookingSystem.Application.Auth.Commands.Login;

public class LoginHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IAuthService _authService;
    private readonly IJwtTokenGenerator _jwt;

    public LoginHandler(IAuthService authService, IJwtTokenGenerator jwt)
    {
        _authService = authService;
        _jwt = jwt;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _authService.ValidateUserAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (user is null)
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        var token = _jwt.GenerateToken(
            user.Id,
            user.Email.Value,
            user.Username,
            user.Role);

        return new AuthResponse
        {
            Token = token,
            Id = user.Id,
            Username = user.Username,
            Email = user.Email.Value,
            Role = user.Role
        };
    }
}

