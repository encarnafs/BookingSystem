using BookingSystem.Api.Mappers;
using BookingSystem.Api.Requests.Users;
using BookingSystem.Application.Users.Commands.CreateUser;
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
    /// <returns>El identificador del usuario creado.</returns>
    /// <remarks>
    /// Solo los administradores pueden crear usuarios.
    /// </remarks>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var command = request.ToCommand();
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    /// <summary>
    /// Actualiza los datos de un usuario existente.
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <param name="request">Datos actualizados del usuario.</param>
    /// <returns>Sin contenido si la operación es exitosa.</returns>
    /// <remarks>
    /// Solo los administradores pueden actualizar usuarios.
    /// </remarks>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        var command = request.ToCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Obtiene todos los usuarios del sistema.
    /// </summary>
    /// <returns>Una colección con todos los usuarios.</returns>
    /// <remarks>
    /// Solo los administradores pueden ver la lista completa de usuarios.
    /// </remarks>
    [HttpGet]
    public async Task<IActionResult> GetAll()
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
    /// Devuelve un objeto con la información del usuario si existe; de lo contrario, un código 404.
    /// </remarks>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id));
        return Ok(result.ToResponse());
    }
}
