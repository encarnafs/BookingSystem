using MediatR;

namespace BookingSystem.Application.Clients.Commands.UpdateClient;

public record UpdateClientCommand(
    Guid Id,
    string FullName,
    string Email,
    string Phone
) : IRequest<Guid>;
