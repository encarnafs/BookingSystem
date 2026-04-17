using BookingSystem.Application.Auth.Commands.Register;
using BookingSystem.Application.Common.Interfaces;
using MediatR;

namespace BookingSystem.Application.Auth.Commands.Register;

public class RegisterHandler : IRequestHandler<RegisterCommand, string>
{
    private readonly IAuthService _authService;
    private readonly IJwtTokenGenerator _jwt;

    public RegisterHandler(IAuthService authService, IJwtTokenGenerator jwt)
    {
        _authService = authService;
        _jwt = jwt;
    }

    public async Task<string> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var userId = await _authService.RegisterAsync(
            request.Username,
            request.Email,
            request.Password,
            cancellationToken);

        return _jwt.GenerateToken(
            userId,
            request.Email,
            request.Username,
            "User");
    }
}
