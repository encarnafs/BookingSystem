using BookingSystem.Application.Auth.Responses;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.ValueObjects;
using MediatR;

namespace BookingSystem.Application.Auth.Commands.RegisterClient;

public class RegisterClientHandler : IRequestHandler<RegisterClientCommand, AuthResponse>
{
    private readonly IAuthService _authService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterClientHandler(IAuthService authService, IJwtTokenGenerator jwtTokenGenerator)
    {
        _authService = authService;
        _jwtTokenGenerator = jwtTokenGenerator;
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
        var hashedPassword = _authService.HashPassword(request.Password);

        var client = new Client(
            request.FullName,
            Email.Create(request.Email),
            PhoneNumber.Create(request.PhoneNumber),
            hashedPassword.Hash,
            hashedPassword.Salt,
            Guid.NewGuid() 
        );


        // 3. Guardar el cliente en la base de datos
        var createdClient = await _authService.CreateClientAsync(client);

        if (createdClient is null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "No se pudo registrar el cliente."
            };
        }

        // 4. Generar el token JWT
        var token = _jwtTokenGenerator.GenerateToken(
            createdClient.Id,
            createdClient.Email.Value,
            createdClient.FullName,
            "Client"
        );

        // 5. Devolver la respuesta de autenticación
        return new AuthResponse
        {
            Success = true,
            Message = "Cliente registrado correctamente.",
            Token = token
        };
    }
}
