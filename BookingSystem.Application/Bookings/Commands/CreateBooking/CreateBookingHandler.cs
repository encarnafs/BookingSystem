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
        // ⭐ 1. Validar que la sala existe
        var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken)
            ?? throw new NotFoundException("Room", request.RoomId);

        // ⭐ 2. Obtener el ID del usuario autenticado
        var createdByUserId = _currentUser.UserId
            ?? throw new ValidationException("Debe estar autenticado para crear una reserva.");

        // ⭐ 3. Obtener el rol del usuario autenticado
        var role = _currentUser.Role;

        // ⭐ 4. Si el rol es Client, el ClientId SIEMPRE es el del usuario autenticado
        //    (un cliente no puede reservar para otro cliente)
        if (role == "Client")
        {
            request.ClientId = createdByUserId;
        }

        // ⭐ 5. Validar que el cliente existe (ya con el ClientId corregido si era Client)
        var client = await _clientRepository.GetByIdAsync(request.ClientId, cancellationToken)
            ?? throw new NotFoundException("Client", request.ClientId);

        // ⭐ 6. Validar que el creador existe en la tabla correcta
        //    - Client → tabla Clients
        //    - User/Admin → tabla Users
        if (role == "Client")
        {
            _ = await _clientRepository.GetByIdAsync(createdByUserId, cancellationToken)
                ?? throw new NotFoundException("Client", createdByUserId);
        }
        else
        {
            _ = await _userRepository.GetByIdAsync(createdByUserId, cancellationToken)
                ?? throw new NotFoundException("User", createdByUserId);
        }

        // ⭐ 7. Validar rango de fechas (DateRange lanza excepción si es inválido)
        var dateRange = new DateRange(request.Start, request.End);

        // ⭐ 8. Comprobar solapamientos con otras reservas
        var overlaps = await _bookingRepository.ExistsOverlappingBookingAsync(
            request.RoomId,
            dateRange.Start,
            dateRange.End,
            null,
            cancellationToken
        );

        if (overlaps)
            throw new BookingOverlapException("La sala ya está reservada para las fechas seleccionadas.");

        // ⭐ 9. Crear la reserva (CreatedAt se rellena automáticamente en el dominio)
        var booking = new Booking(
            room.Id,
            client.Id,
            createdByUserId,
            dateRange,
            request.Comments
        );

        await _bookingRepository.AddAsync(booking, cancellationToken);

        // ⭐ 10. Guardar cambios
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Recargar con navegación
        var created = await _bookingRepository.GetByIdAsync(booking.Id, cancellationToken);

        // ⭐ 11. Publicar evento de dominio
        await _mediator.Publish(new BookingCreatedNotification(booking.Id), cancellationToken);

        // ⭐ 12. Devolver DTO
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
            CreatedAt = booking.CreatedAt,
            RoomName = created.Room!.Name,
            ClientFullName = created.Client!.FullName
        };
    }
}