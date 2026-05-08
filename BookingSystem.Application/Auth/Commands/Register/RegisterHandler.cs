using BookingSystem.Application.Auth.Commands.Register;
using BookingSystem.Application.Auth.Responses;
using BookingSystem.Application.Common.Interfaces;
using MediatR;

namespace BookingSystem.Application.Auth.Commands.Register;

public class RegisterHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IAuthService _authService;
    private readonly IJwtTokenGenerator _jwt;

    public RegisterHandler(IAuthService authService, IJwtTokenGenerator jwt)
    {
        _authService = authService;
        _jwt = jwt;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = await _authService.RegisterAsync(
            request.Username,
            request.Email,
            request.Password,
            cancellationToken);

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
