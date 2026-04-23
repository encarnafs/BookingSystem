using BookingSystem.Application.Bookings.Commands.CreateBooking;
using BookingSystem.Application.Bookings.Dtos;
using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.ValueObjects;
using MediatR;

namespace BookingSystem.Application.Bookings.Commands.CreateBooking;
public class CreateBookingHandler : IRequestHandler<CreateBookingCommand, BookingDto>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;

    public CreateBookingHandler(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository,
        IClientRepository clientRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUser)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _clientRepository = clientRepository;
        _userRepository = userRepository;
        _currentUser = currentUser;
    }

    public async Task<BookingDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (room is null) throw new NotFoundException("Room", request.RoomId);

        var client = await _clientRepository.GetByIdAsync(request.ClientId, cancellationToken);
        if (client is null) throw new NotFoundException("Client", request.ClientId);

        var createdByUserId =
            Guid.TryParse(_currentUser.UserId, out var parsedUserId)
                ? parsedUserId
                : Guid.Empty;

        if (createdByUserId != Guid.Empty)
        {
            var user = await _userRepository.GetByIdAsync(createdByUserId, cancellationToken);
            if (user is null) throw new NotFoundException("User", createdByUserId);
        }

        if (request.Start >= request.End)
            throw new ValidationException("Fecha Start debe ser antes de Fecha End");

        var dateRange = new DateRange(request.Start, request.End);

        bool overlaps = await _bookingRepository.ExistsOverlappingBookingAsync(
            request.RoomId,
            request.Start,
            request.End,
            cancellationToken
        );

        if (overlaps)
            throw new ValidationException("La sala ya está reservada para las fechas seleccionadas");

        var booking = new Booking(
            room.Id,
            client.Id,
            createdByUserId,
            dateRange,
            request.Comments
        );

        await _bookingRepository.AddAsync(booking, cancellationToken);

        return new BookingDto
        {
            Id = booking.Id,
            RoomId = booking.RoomId,
            ClientId = booking.ClientId,
            CreatedByUserId = booking.CreatedByUserId,
            Start = booking.DateRange.Start,
            End = booking.DateRange.End,
            Comments = booking.Comments,
            Status = booking.Status.ToString(),
            CreatedAt = booking.CreatedAt
        };

    }
}

