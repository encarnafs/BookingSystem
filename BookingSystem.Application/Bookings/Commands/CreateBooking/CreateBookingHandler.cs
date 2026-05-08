using BookingSystem.Application.Bookings.Dtos;
using BookingSystem.Application.Bookings.Events;
using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Exceptions;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public CreateBookingHandler(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository,
        IClientRepository clientRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _clientRepository = clientRepository;
        _userRepository = userRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<BookingDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken)
            ?? throw new NotFoundException("Room", request.RoomId);

        var client = await _clientRepository.GetByIdAsync(request.ClientId, cancellationToken)
            ?? throw new NotFoundException("Client", request.ClientId);

        var createdByUserId = _currentUser.UserId
            ?? throw new ValidationException("Debe estar autenticado para crear una reserva.");

        _ = await _userRepository.GetByIdAsync(createdByUserId, cancellationToken)
            ?? throw new NotFoundException("User", createdByUserId);

        var dateRange = new DateRange(request.Start, request.End);

        var overlaps = await _bookingRepository.ExistsOverlappingBookingAsync(
            request.RoomId,
            dateRange.Start,
            dateRange.End,
            null,
            cancellationToken
        );

        if (overlaps)
            throw new BookingOverlapException("La sala ya está reservada para las fechas seleccionadas.");

        var booking = new Booking(
            room.Id,
            client.Id,
            createdByUserId,
            dateRange,
            request.Comments
        );

        await _bookingRepository.AddAsync(booking, cancellationToken);

        // ⭐ GUARDAR CAMBIOS
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // ⭐ PUBLICAR APPLICATION EVENT
        await _mediator.Publish(new BookingCreatedNotification(booking.Id), cancellationToken);

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

