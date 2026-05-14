using BookingSystem.Application.Auth.Responses;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.ValueObjects;
using MediatR;

namespace BookingSystem.Application.Auth.Commands.RegisterUser;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, AuthResponse>
{
    private readonly IAuthService _authService;
    private readonly IJwtTokenGenerator _jwt;

    public RegisterUserHandler(IAuthService authService, IJwtTokenGenerator jwt)
    {
        _authService = authService;
        _jwt = jwt;
    }

    public async Task<AuthResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Comprobar si el email ya existe
        var existingUser = await _authService.GetUserByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "El email ya está registrado."
            };
        }

        // 2. Hashear contraseña
        var hashed = _authService.HashPassword(request.Password);

        // 3. Crear entidad User (DDD)
        var user = new User(
            request.Username,
            Email.Create(request.Email),
            hashed.Hash,
            hashed.Salt,
            "User"
        );

        // 4. Guardar en base de datos
        var createdUser = await _authService.CreateUserAsync(user);

        if (createdUser is null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "No se pudo registrar el usuario."
            };
        }

        // 5. Generar Token JWT
        var token = _jwt.GenerateToken(
            createdUser.Id,
            createdUser.Email.Value,
            createdUser.Username,
            createdUser.Role
        );

        // 6. Devolver respuesta final
        return new AuthResponse
        {
            Success = true,
            Message = "Usuario registrado correctamente.",
            Token = token,
            Id = createdUser.Id,
            Username = createdUser.Username,
            Email = createdUser.Email.Value,
            Role = createdUser.Role
        };
    }
}
