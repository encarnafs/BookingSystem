using BookingSystem.Api.Requests.Clients;
using BookingSystem.Api.Responses.Clients;
using BookingSystem.Application.Clients.Commands.CreateClient;
using BookingSystem.Application.Clients.Commands.UpdateClient;
using BookingSystem.Application.Clients.Dtos;

namespace BookingSystem.Api.Mappers;

public static class ClientMapper
{
    public static CreateClientCommand ToCommand(this CreateClientRequest request)
    => new(request.FullName, request.Email, request.Phone, request.Password);

    public static UpdateClientCommand ToCommand(this UpdateClientRequest request, Guid id)
        => new(id, request.FullName, request.Email, request.Phone);

    public static ClientResponse ToResponse(this ClientDto dto)
        => new()
        {
            Id = dto.Id,
            FullName = dto.FullName,
            Email = dto.Email,
            Phone = dto.PhoneNumber
        };
}
