using BookingSystem.Application.Auth.Responses;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BookingSystem.Application.Auth.Commands.RegisterClient;

public class RegisterClientHandler : IRequestHandler<RegisterClientCommand, AuthResponse>
{
    private readonly IAuthService _authService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private ICurrentUserService _currentUserService;
    private readonly IPasswordHasher<Client> _passwordHasher;

    public RegisterClientHandler(IAuthService authService, IJwtTokenGenerator jwtTokenGenerator, ICurrentUserService currentUserService, IPasswordHasher<Client> passwordHasher)
    {
        _authService = authService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _currentUserService = currentUserService;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResponse> Handle(RegisterClientCommand request, CancellationToken cancellationToken)
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

        // 2. Crear el cliente con contraseña hasheada
        var client = new Client(
            request.FullName,
            Email.Create(request.Email),
            PhoneNumber.Create(request.PhoneNumber)
        );

        // 3. Hashear la contraseña con el estándar
        var hashedPassword = _passwordHasher.HashPassword(client, request.Password);
        client.SetPassword(hashedPassword);

        // 4️. Asignar CreatedByUserId correctamente
        var createdBy = _currentUserService.UserId ?? client.Id;
        client.SetCreatedBy(createdBy);

        // 5️. Guardar el cliente en la base de datos
        var createdClient = await _authService.CreateClientAsync(client);
        if (createdClient is null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "No se pudo registrar el cliente."
            };
        }

        // 6️. Generar el token JWT
        var token = _jwtTokenGenerator.GenerateToken(
            createdClient.Id,
            createdClient.Email.Value,
            createdClient.FullName,
            "Client"
        );

        // 7️. Devolver la respuesta
        return new AuthResponse
        {
            Success = true,
            Id = createdClient.Id,
            Username = createdClient.FullName,
            Email = createdClient.Email.Value,
            Role = "Client",
            Token = token,
            Message = "Cliente registrado correctamente."
        };

    }
}
