using BookingSystem.Api.Requests.Auth;
using BookingSystem.Api.Responses.Auth;
using BookingSystem.Application.Auth.Commands.Login;
using BookingSystem.Application.Auth.Commands.Register;
using BookingSystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var token = await _mediator.Send(
            new RegisterCommand(request.Username, request.Email, request.Password));

        return Ok(new { Token = token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var token = await _mediator.Send(
            new LoginCommand(request.Email, request.Password));

        return Ok(new { Token = token });
    }

    // Este endpoint es solo para fines de prueba para verificar que la autenticación funciona y para ver la información del usuario actual.
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me([FromServices] ICurrentUserService currentUser)
    {
        if (currentUser.UserId is null)
            return Unauthorized();

        var response = new UserProfileResponse
        {
            UserId = currentUser.UserId,
            Email = currentUser.Email ?? string.Empty,
            Role = currentUser.Role ?? string.Empty
        };

        return Ok(response);
    }

}
