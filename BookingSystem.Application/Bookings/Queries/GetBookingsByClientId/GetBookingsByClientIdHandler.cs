using BookingSystem.Application.Bookings.Dtos;
using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using MediatR;

namespace BookingSystem.Application.Bookings.Queries.GetBookingsByClientId;

public class GetBookingsByClientIdHandler
    : IRequestHandler<GetBookingsByClientIdQuery, List<BookingDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IClientRepository _clientRepository;

    public GetBookingsByClientIdHandler(
        IBookingRepository bookingRepository,
        IClientRepository clientRepository)
    {
        _bookingRepository = bookingRepository;
        _clientRepository = clientRepository;
    }

    public async Task<List<BookingDto>> Handle(
        GetBookingsByClientIdQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Validar que el cliente existe
        var client = await _clientRepository.GetByIdAsync(request.ClientId, cancellationToken);
        if (client is null)
            throw new NotFoundException("Client", request.ClientId);

        // 2. Obtener reservas del cliente
        var bookings = await _bookingRepository.GetByClientAsync(request.ClientId, cancellationToken);

        // 3. Mapear a DTO
        return bookings
            .Select(b => new BookingDto
            {
                Id = b.Id,
                RoomId = b.RoomId,
                ClientId = b.ClientId,
                CreatedByUserId = b.CreatedByUserId,
                Start = b.DateRange.Start,
                End = b.DateRange.End,
                Comments = b.Comments,
                Status = b.Status.ToString(),
                CreatedAt = b.CreatedAt
            })
            .ToList();
    }
}
