using BookingSystem.Application.Auth.Responses;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BookingSystem.Application.Auth.Commands.RegisterUser;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, AuthResponse>
{
    private readonly IAuthService _authService;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IPasswordHasher<User> _passwordHasher;

    public RegisterUserHandler(
        IAuthService authService,
        IJwtTokenGenerator jwt,
        IPasswordHasher<User> passwordHasher)
    {
        _authService = authService;
        _jwt = jwt;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // 1️. Comprobar si el email ya existe
        var existingUser = await _authService.GetUserByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "El email ya está registrado."
            };
        }

        // 2️. Determinar si es el primer usuario del sistema
        var totalUsers = await _authService.CountUsersAsync();

        // Bloquear registro público si ya existe un usuario
        if (totalUsers > 0)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "El registro público está deshabilitado. Solo un administrador puede crear nuevos usuarios."
            };
        }

        var role = "Admin"; // Primer usuario siempre Admin

        // 3️. Crear entidad User sin contraseña
        var user = new User(
            request.Username,
            Email.Create(request.Email),
            role
        );

        // 4️. Hashear contraseña
        var hashedPassword = _passwordHasher.HashPassword(user, request.Password);
        user.SetPassword(hashedPassword);

        // 5️. Guardar en BD
        var createdUser = await _authService.CreateUserAsync(user);

        if (createdUser is null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "No se pudo registrar el usuario."
            };
        }

        // 6️. Generar JWT
        var token = _jwt.GenerateToken(
            createdUser.Id,
            createdUser.Email.Value,
            createdUser.Username,
            createdUser.Role
        );

        // 7️. Respuesta final
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