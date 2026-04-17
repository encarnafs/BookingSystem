using BookingSystem.Api.Requests.Auth;
using BookingSystem.Application.Auth.Commands.Login;
using BookingSystem.Application.Auth.Commands.Register;
using MediatR;
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
}
