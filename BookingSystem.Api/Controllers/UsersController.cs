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
    /// <remarks>Solo los administradores pueden crear usuarios.</remarks>
    [HttpPost]
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
    /// <remarks>Solo los administradores pueden actualizar usuarios.</remarks>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        var command = request.ToCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Cambia la contraseña de un usuario.
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <param name="request">Nueva contraseña del usuario.</param>
    /// <returns>Sin contenido si la operación es exitosa.</returns>
    /// <remarks>Solo los administradores pueden cambiar contraseñas.</remarks>
    [HttpPut("{id:guid}/change-password")]
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
    /// <param name="request">Nuevo rol que se asignará al usuario.</param>
    /// <returns>Sin contenido si la operación es exitosa.</returns>
    /// <remarks>Solo los administradores pueden cambiar roles.</remarks>
    [HttpPut("{id:guid}/change-role")]
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
    /// <remarks>El usuario no podrá iniciar sesión tras ser desactivado.</remarks>
    [HttpPut("{id:guid}/disable")]
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
    /// <remarks>El usuario podrá iniciar sesión tras ser activado.</remarks>
    [HttpPut("{id:guid}/enable")]
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
    /// <remarks>El usuario se marca como eliminado y deja de estar disponible.</remarks>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteUserCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Obtiene todos los usuarios del sistema.
    /// </summary>
    /// <returns>Una colección con todos los usuarios activos y no eliminados.</returns>
    /// <remarks>Solo los administradores pueden ver la lista completa de usuarios.</remarks>
    [HttpGet]
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
    /// <remarks>Devuelve 404 si el usuario no existe.</remarks>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id));

        if (result is null)
            return NotFound();

        return Ok(result.ToResponse());
    }
}