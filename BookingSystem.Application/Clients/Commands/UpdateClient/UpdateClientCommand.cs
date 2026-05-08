using BookingSystem.Application.Clients.Dtos;
using MediatR;

namespace BookingSystem.Application.Clients.Commands.UpdateClient;

public record UpdateClientCommand(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber
) : IRequest<ClientDto>;
