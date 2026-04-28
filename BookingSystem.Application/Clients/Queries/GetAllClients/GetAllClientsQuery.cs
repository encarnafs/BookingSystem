using BookingSystem.Application.Clients.Dtos;
using MediatR;

namespace BookingSystem.Application.Clients.Queries.GetAllClients;

public record GetAllClientsQuery() : IRequest<List<ClientDto>>;
