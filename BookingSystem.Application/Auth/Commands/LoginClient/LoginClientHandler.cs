using BookingSystem.Application.Auth.Responses;
using BookingSystem.Application.Common.Interfaces;
using MediatR;

namespace BookingSystem.Application.Auth.Commands.LoginClient;

public class LoginClientHandler : IRequestHandler<LoginClientCommand, AuthResponse>
{
    private readonly IAuthService _authService;
    private readonly IJwtTokenGenerator _jwt;

    public LoginClientHandler(IAuthService authService, IJwtTokenGenerator jwt)
    {
        _authService = authService;
        _jwt = jwt;
    }

    public async Task<AuthResponse> Handle(LoginClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _authService.ValidateClientAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (client is null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Credenciales inválidas."
            };
        }

        var token = _jwt.GenerateToken(
            client.Id,
            client.Email.Value,
            client.FullName,
            client.Role);

        return new AuthResponse
        {
            Success = true,
            Message = "Inicio de sesión correcto.",
            Token = token,
            Id = client.Id,
            Username = client.FullName,
            Email = client.Email.Value,
            Role = client.Role
        };
    }
}
