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
    /// Registra un nuevo usuario interno del sistema (User o Admin).
    /// </summary>
    /// <param name="request">Datos necesarios para registrar el usuario.</param>
    /// <returns>Información del usuario registrado junto con el token JWT.</returns>
    /// <remarks>
    /// Reglas de negocio:
    /// - El email debe ser único.
    /// - La contraseña debe cumplir los requisitos mínimos.
    /// - Devuelve <b>200 OK</b> con el token JWT del usuario recién creado.
    ///
    /// Seguridad:
    /// - Endpoint público (AllowAnonymous).
    /// </remarks>
    /// <response code="200">Usuario registrado correctamente.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="409">Conflicto: el email ya está registrado.</response>
    /// <response code="500">Error interno del servidor.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("register-user")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponse>> RegisterUser(RegisterRequest request)
    {
        var response = await _mediator.Send(
            new RegisterUserCommand(request.Username, request.Email, request.Password));

        return Ok(response);
    }

    /// <summary>
    /// Registra un nuevo cliente del sistema.
    /// </summary>
    /// <param name="request">Datos necesarios para registrar el cliente.</param>
    /// <returns>Información del cliente registrado junto con el token JWT.</returns>
    /// <remarks>
    /// Reglas de negocio:
    /// - El email debe ser único.
    /// - Los clientes autenticados no pueden registrar otros clientes.
    /// - Devuelve <b>200 OK</b> con el token JWT del cliente recién creado.
    ///
    /// Seguridad:
    /// - Endpoint público (AllowAnonymous).
    /// - Si un usuario autenticado con rol Client intenta registrar, se devuelve 401.
    /// </remarks>
    /// <response code="200">Cliente registrado correctamente.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="401">No autorizado: un cliente autenticado no puede registrar otros clientes.</response>
    /// <response code="409">Conflicto: el email ya está registrado.</response>
    /// <response code="500">Error interno del servidor.</response>
    [AllowAnonymous]
    [HttpPost("register-client")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponse>> RegisterClient(RegisterRequest request)
    {
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
    /// <param name="request">Credenciales del usuario.</param>
    /// <returns>Token JWT si las credenciales son correctas.</returns>
    /// <remarks>
    /// Reglas de negocio:
    /// - El usuario debe existir.
    /// - La contraseña debe ser válida.
    ///
    /// Seguridad:
    /// - Endpoint público (AllowAnonymous).
    /// </remarks>
    /// <response code="200">Inicio de sesión correcto.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="401">Credenciales incorrectas.</response>
    /// <response code="500">Error interno del servidor.</response>
    [AllowAnonymous]
    [HttpPost("login-user")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
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
    /// <param name="request">Credenciales del cliente.</param>
    /// <returns>Token JWT si las credenciales son correctas.</returns>
    /// <remarks>
    /// Reglas de negocio:
    /// - El cliente debe existir.
    /// - La contraseña debe ser válida.
    ///
    /// Seguridad:
    /// - Endpoint público (AllowAnonymous).
    /// </remarks>
    /// <response code="200">Inicio de sesión correcto.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="401">Credenciales incorrectas.</response>
    /// <response code="500">Error interno del servidor.</response>
    [AllowAnonymous]
    [HttpPost("login-client")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
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
    /// <returns>Datos básicos del usuario autenticado.</returns>
    /// <remarks>
    /// Reglas de negocio:
    /// - Requiere un token JWT válido.
    /// - Devuelve <b>200 OK</b> con la información del usuario.
    ///
    /// Seguridad:
    /// - Requiere autenticación (Authorize).
    /// </remarks>
    /// <response code="200">Información del usuario devuelta correctamente.</response>
    /// <response code="401">No autorizado.</response>
    [Authorize]
    [HttpGet("me")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
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