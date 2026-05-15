using BookingSystem.Api.Requests.Auth;
using BookingSystem.Api.Responses.Auth;
using BookingSystem.Application.Auth.Commands.LoginClient;
using BookingSystem.Application.Auth.Commands.LoginUser;
using BookingSystem.Application.Auth.Commands.RegisterClient;
using BookingSystem.Application.Auth.Commands.RegisterUser;
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
    [Authorize(Roles = "Admin")]
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
    [AllowAnonymous]
    [HttpPost("register-client")]
    public async Task<ActionResult<AuthResponse>> RegisterClient(RegisterRequest request)
    {
        // Si el usuario está autenticado como CLIENTE → bloquear
        if (User.Identity?.IsAuthenticated == true &&
            User.IsInRole("Client"))
        {
            return Unauthorized(new AuthResponse
            {
                Success = false,
                Message = "Los clientes no pueden registrar otros clientes."
            });
        }

        var response = await _mediator.Send(
            new RegisterClientCommand(request.FullName, request.Email, request.PhoneNumber, request.Password));

        return Ok(response);
    }

    /// <summary>
    /// Inicia sesión como usuario interno (Admin o User).
    /// </summary>
    [HttpPost("login-user")]
    public async Task<ActionResult<AuthResponse>> LoginUser(LoginRequest request)
    {
        var response = await _mediator.Send(
            new LoginUserCommand(request.Email, request.Password));

        if (!response.Success)
            return Unauthorized(response);

        return Ok(response);
    }

    /// <summary>
    /// Inicia sesión como cliente.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login-client")]
    public async Task<ActionResult<AuthResponse>> LoginClient(LoginRequest request)
    {
        var response = await _mediator.Send(
            new LoginClientCommand(request.Email, request.Password));

        if (!response.Success)
            return Unauthorized(response);

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

