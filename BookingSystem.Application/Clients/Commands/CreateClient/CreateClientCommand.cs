using BookingSystem.Application.Clients.Dtos;
using BookingSystem.Domain.ValueObjects;
using MediatR;

namespace BookingSystem.Application.Clients.Commands.CreateClient;

public record CreateClientCommand(
    string FullName,
    string Email,
    string PhoneNumber
) : IRequest<ClientDto>;
