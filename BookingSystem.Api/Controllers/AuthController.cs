using BookingSystem.Api.Requests.Auth;
using BookingSystem.Api.Responses.Auth;
using BookingSystem.Application.Auth.Commands.Login;
using BookingSystem.Application.Auth.Commands.RegisterUser;
using BookingSystem.Application.Auth.Commands.RegisterClient;
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

    /// <summary>
    /// Registra un nuevo usuario del sistema (User).
    /// </summary>
    [HttpPost("register-user")]
    public async Task<ActionResult<AuthResponse>> RegisterUser(RegisterRequest request)
    {
        var response = await _mediator.Send(
            new RegisterUserCommand(request.Username, request.Email, request.Password));

        return Ok(response);
    }

    /// <summary>
    /// Registra un nuevo cliente (Client).
    /// </summary>
    [HttpPost("register-client")]
    public async Task<ActionResult<AuthResponse>> RegisterClient(RegisterRequest request)
    {
        var response = await _mediator.Send(
            new RegisterClientCommand(request.FullName, request.Email, request.PhoneNumber, request.Password));

        return Ok(response);
    }

    /// <summary>
    /// Inicia sesión y obtiene un token JWT.
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var response = await _mediator.Send(
            new LoginCommand(request.Email, request.Password));

        return Ok(response);
    }

    /// <summary>
    /// Obtiene la información del usuario autenticado.
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public ActionResult<UserProfileResponse> Me([FromServices] ICurrentUserService currentUser)
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

