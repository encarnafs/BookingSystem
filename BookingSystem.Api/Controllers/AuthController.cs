using BookingSystem.Api.Requests.Auth;
using BookingSystem.Api.Responses.Auth;
using BookingSystem.Application.Auth.Commands.Login;
using BookingSystem.Application.Auth.Commands.Register;
using BookingSystem.Application.Auth.Responses;
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
        AuthResponse response = await _mediator.Send(
            new RegisterCommand(request.Username, request.Email, request.Password));

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        AuthResponse response = await _mediator.Send(
            new LoginCommand(request.Email, request.Password));

        return Ok(response);
    }

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

