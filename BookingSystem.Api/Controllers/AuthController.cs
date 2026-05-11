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

    /// <summary>
    /// Registra un nuevo usuario en el sistema.
    /// </summary>
    /// <param name="request">Datos necesarios para registrar al usuario.</param>
    /// <returns>Los datos del usuario registrado, incluyendo el token de autenticación.</returns>
    /// <remarks>
    /// Este endpoint crea un usuario con rol por defecto "Client".
    /// </remarks>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        AuthResponse response = await _mediator.Send(
            new RegisterCommand(request.Username, request.Email, request.Password));

        return Ok(response);
    }

    /// <summary>
    /// Inicia sesión y obtiene un token JWT.
    /// </summary>
    /// <param name="request">Credenciales del usuario (email y contraseña).</param>
    /// <returns>Los datos del usuario autenticado, incluyendo el token JWT.</returns>
    /// <remarks>
    /// El token devuelto debe enviarse en el header Authorization para acceder a endpoints protegidos.
    /// </remarks>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        AuthResponse response = await _mediator.Send(
            new LoginCommand(request.Email, request.Password));

        return Ok(response);
    }

    /// <summary>
    /// Obtiene la información del usuario actualmente autenticado.
    /// </summary>
    /// <param name="currentUser">Servicio que contiene los datos del usuario autenticado.</param>
    /// <returns>El perfil del usuario autenticado.</returns>
    /// <remarks>
    /// Requiere un token JWT válido.  
    /// Devuelve el ID, email y rol del usuario actual.
    /// </remarks>
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
