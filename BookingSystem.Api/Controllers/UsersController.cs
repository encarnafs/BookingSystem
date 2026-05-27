using BookingSystem.Api.Mappers;
using BookingSystem.Api.Requests.Users;
using BookingSystem.Api.Responses.Users;
using BookingSystem.Application.Users.Commands.ChangeUserPassword;
using BookingSystem.Application.Users.Commands.ChangeUserRole;
using BookingSystem.Application.Users.Commands.CreateUser;
using BookingSystem.Application.Users.Commands.DeleteUser;
using BookingSystem.Application.Users.Commands.DisableUser;
using BookingSystem.Application.Users.Commands.EnableUser;
using BookingSystem.Application.Users.Commands.UpdateUser;
using BookingSystem.Application.Users.Queries.GetAllUsers;
using BookingSystem.Application.Users.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Crea un nuevo usuario en el sistema.
    /// </summary>
    /// <param name="request">Datos necesarios para crear el usuario.</param>
    /// <returns>El usuario creado.</returns>
    /// <remarks>
    /// Reglas de autorización:
    /// - Solo los administradores pueden crear usuarios.
    ///  
    /// Reglas de negocio:
    /// - El email es obligatorio y debe ser único.
    /// - El rol debe ser válido (Admin, User, Client).
    /// - Si el email ya existe, se devuelve 409 Conflict.
    /// </remarks>
    /// <b>Respuestas:</b>
    /// <response code="201">Usuario creado correctamente.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido.</response>
    /// <response code="409">Conflicto: el email ya está registrado.</response>
    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request)
    {
        var userDto = await _mediator.Send(request.ToCommand());
        return CreatedAtAction(nameof(GetById), new { id = userDto.Id }, userDto.ToResponse());
    }

    /// <summary>
    /// Actualiza los datos de un usuario existente.
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <param name="request">Datos actualizados del usuario.</param>
    /// <returns>Sin contenido si la operación es exitosa.</returns>
    /// <remarks>
    /// Reglas de autorización:
    /// - Solo los administradores pueden actualizar usuarios.
    ///
    /// Reglas de negocio:
    /// - El usuario debe existir.
    /// - El email es obligatorio y debe ser único.
    /// - El rol debe ser válido (Admin, User, Client).
    /// - Si el usuario no existe, se devuelve 404 NotFound.
    /// - Si el email ya está registrado por otro usuario, se devuelve 409 Conflict..
    /// </remarks>
    /// <b>Respuestas:</b>
    /// <response code="204">Usuario actualizado correctamente.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido.</response>
    /// <response code="404">Usuario no encontrado.</response>
    /// <response code="409">Conflicto: el email ya está registrado.</response>
    [HttpPut("{id:guid}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        var command = request.ToCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Cambia la contraseña de un usuario.
    /// </summary>
    /// <remarks>
    /// Reglas de autorización:
    /// - Solo administradores.
    ///
    /// Reglas de negocio:
    /// - El usuario debe existir.
    /// - La contraseña actual debe ser correcta.
    /// - La nueva contraseña debe cumplir los requisitos mínimos.
    /// - Si el usuario no existe, se devuelve 404 NotFound.
    /// - Si la contraseña actual no coincide, se devuelve 409 Conflict.
    /// </remarks>
    /// <b>Respuestas:</b>
    /// <response code="204">Contraseña cambiada correctamente.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido.</response>
    /// <response code="404">Usuario no encontrado.</response>
    /// <response code="409">La contraseña actual no es correcta.</response>
    [HttpPut("{id:guid}/change-password")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangeUserPasswordRequest request)
    {
        var command = new ChangeUserPasswordCommand(id, request.CurrentPassword, request.NewPassword);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Cambia el rol de un usuario.
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <param name="request">Nuevo rol del usuario.</param>
    /// <returns>Sin contenido si la operación es exitosa.</returns>
    /// <remarks>
    /// Reglas de autorización:
    /// - Solo administradores.
    ///
    /// Reglas de negocio:
    /// - El usuario debe existir.
    /// - El rol debe ser válido (Admin, User, Client).
    /// - Si el usuario no existe, se devuelve 404 NotFound.
    /// - Si el rol es igual al actual, se devuelve 409 Conflict.
    /// </remarks>
    /// <b>Respuestas:</b>
    /// <response code="204">Rol cambiado correctamente.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido.</response>
    /// <response code="404">Usuario no encontrado.</response>
    /// <response code="409">El rol ya coincide con el actual.</response>
    [HttpPut("{id:guid}/change-role")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ChangeRole(Guid id, [FromBody] ChangeUserRoleRequest request)
    {
        var command = new ChangeUserRoleCommand(id, request.NewRole);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Desactiva un usuario (IsActive = false).
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <returns>Sin contenido si la operación es exitosa.</returns>
    /// <remarks>
    /// Reglas de autorización:
    /// - Solo administradores.
    ///
    /// Reglas de negocio:
    /// - El usuario debe existir.
    /// - El usuario debe estar activo para poder desactivarlo.
    /// - Si el usuario no existe, se devuelve 404 NotFound.
    /// - Si el usuario ya está desactivado, se devuelve 409 Conflict.
    /// </remarks>
    /// <b>Respuestas:</b>
    /// <response code="204">Usuario desactivado correctamente.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido.</response>
    /// <response code="404">Usuario no encontrado.</response>
    /// <response code="409">El usuario ya está desactivado.</response>
    [HttpPut("{id:guid}/disable")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Disable(Guid id)
    {
        await _mediator.Send(new DisableUserCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Activa un usuario (IsActive = true).
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <returns>Sin contenido si la operación es exitosa.</returns>
    /// <remarks>
    /// Reglas de autorización:
    /// - Solo administradores.
    ///
    /// Reglas de negocio:
    /// - El usuario debe existir.
    /// - El usuario debe estar desactivado para poder activarlo.
    /// - Si el usuario no existe, se devuelve 404 NotFound.
    /// - Si el usuario ya está activo, se devuelve 409 Conflict.
    /// </remarks>
    /// <b>Respuestas:</b>
    /// <response code="204">Usuario activado correctamente.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido.</response>
    /// <response code="404">Usuario no encontrado.</response>
    /// <response code="409">El usuario ya está activo.</response>
    [HttpPut("{id:guid}/enable")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> EnableUser(Guid id)
    {
        await _mediator.Send(new EnableUserCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Elimina un usuario del sistema (soft delete).
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <returns>Sin contenido si la operación es exitosa.</returns>
    /// <remarks>
    /// Reglas de autorización:
    /// - Solo administradores.
    ///
    /// Reglas de negocio:
    /// - El usuario debe existir.
    /// - La eliminación es lógica (soft delete).
    /// - El usuario debe estar activo/no eliminado para poder eliminarlo.
    /// - Si el usuario no existe, se devuelve 404 NotFound.
    /// - Si el usuario ya está eliminado, se devuelve 409 Conflict.
    /// </remarks>
    /// <b>Respuestas:</b>
    /// <response code="204">Usuario eliminado correctamente.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido.</response>
    /// <response code="404">Usuario no encontrado.</response>
    /// <response code="409">El usuario ya está eliminado.</response>
    [HttpDelete("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteUserCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Obtiene todos los usuarios del sistema.
    /// </summary>
    /// <returns>Una colección con todos los usuarios activos y no eliminados.</returns>
    /// <remarks>
    /// Reglas de autorización:
    /// - Solo administradores.
    ///
    /// Notas:
    /// - Si no existen usuarios, se devuelve una lista vacía con código 200.
    /// </remarks>
    /// <b>Respuestas:</b>
    /// <response code="200">Listado devuelto correctamente.</response>
    /// <response code="400">Parámetros inválidos.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido.</response>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllUsersQuery());
        return Ok(result.Select(u => u.ToResponse()));
    }

    /// <summary>
    /// Obtiene un usuario por su identificador único.
    /// </summary>
    /// <param name="id">Identificador del usuario (GUID).</param>
    /// <returns>Objeto con los datos del usuario.</returns>
    /// <remarks>
    /// Reglas de autorización:
    /// - Solo administradores.
    ///
    /// Reglas de negocio:
    /// - Devuelve 404 si el usuario no existe.
    /// </remarks>
    /// <b>Respuestas:</b>
    /// <response code="200">Usuario encontrado.</response>
    /// <response code="400">Parámetros inválidos.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido.</response>
    /// <response code="404">Usuario no encontrado.</response>
    [HttpGet("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id));

        if (result is null)
            return NotFound();

        return Ok(result.ToResponse());
    }
}