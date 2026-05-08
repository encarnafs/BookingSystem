using MediatR;

namespace BookingSystem.Application.Clients.Commands.DisableClient;

public record DisableClientCommand(Guid ClientId) : IRequest<Unit>;
