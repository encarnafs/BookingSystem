using BookingSystem.Domain.Enums;
using BookingSystem.Domain.Events;
using BookingSystem.Domain.ValueObjects;
using BookingSystem.Domain.Abstractions;
using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Domain.Entities;

public class Booking: IHasDomainEvents
{
    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateRange DateRange { get; private set; } = default!;
    public BookingStatus Status { get; private set; }
    public string? Comments { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // ⭐⭐ PROPIEDADES DE NAVEGACIÓN
    public Room Room { get; private set; } = default!;
    public Client Client { get; private set; } = default!;


    //DOMAIN EVENTS IMPLEMENTATION
    // ⭐⭐ 1. Lista interna de Domain Events
    private readonly List<object> _domainEvents = new();

    // ⭐⭐ 2. Exponerlos como solo lectura
    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    // ⭐⭐ 3. Método para añadir eventos
    protected void AddDomainEvent(object eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    // ⭐ Método para limpiarlos (lo usará EF Core)
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
    //

    // Constructor privado para EF Core
    private Booking() { }


    // Constructor principal
    public Booking(Guid roomId, Guid clientId, Guid createdByUserId, DateRange dateRange, string? comments = null)
    {
        if (roomId == Guid.Empty)
            throw new InvalidBookingStateException("El RoomId no puede estar vacío.");

        if (clientId == Guid.Empty)
            throw new InvalidBookingStateException("El ClientId no puede estar vacío.");

        if (createdByUserId == Guid.Empty)
            throw new InvalidBookingStateException("El CreatedByUserId no puede estar vacío.");

        if (dateRange is null)
            throw new InvalidBookingStateException("El DateRange no puede ser nulo.");


        Id = Guid.NewGuid();
        RoomId = roomId;
        ClientId = clientId;
        CreatedByUserId = createdByUserId;
        DateRange = dateRange;
        Comments = comments;
        Status = BookingStatus.Pending;
        CreatedAt = DateTime.UtcNow;

        // ⭐ Aquí emitimos el Domain Event (sólo se registra, no se publica)
        AddDomainEvent(new BookingCreatedEvent(Id));
    }

    public void UpdateDates(DateRange newDateRange)
    {
        if (Status == BookingStatus.Cancelled)
            throw new InvalidBookingStateException("No se pueden modificar fechas de una reserva cancelada.");

        if (DateRange.End < DateTime.UtcNow)
            throw new InvalidBookingStateException("No se puede modificar una reserva que ya ha finalizado.");

        DateRange = newDateRange;
    }

    public void UpdateComments(string? comments)
    {
        if (Status == BookingStatus.Cancelled)
            throw new InvalidBookingStateException("No se pueden modificar comentarios de una reserva cancelada.");

        Comments = comments;
    }


    public void Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            throw new BookingAlreadyCancelledException(Id);

        Status = BookingStatus.Cancelled;
    }

    public void Confirm()
    {
        if (Status == BookingStatus.Cancelled)
            throw new InvalidBookingStateException("No se puede confirmar una reserva cancelada.");

        if (Status == BookingStatus.Confirmed)
            throw new BookingAlreadyConfirmedException(Id);

        Status = BookingStatus.Confirmed;
    }


    public void Update(Guid roomId, Guid clientId, DateRange newDateRange, string? comments)
    {
        if (Status == BookingStatus.Cancelled)
            throw new InvalidBookingStateException("No se puede actualizar una reserva cancelada.");

        if (DateRange.End < DateTime.UtcNow)
            throw new InvalidBookingStateException("No se puede modificar una reserva que ya ha finalizado");

        if (roomId == Guid.Empty)
            throw new InvalidBookingStateException("El RoomId no puede estar vacío.");

        if (clientId == Guid.Empty)
            throw new InvalidBookingStateException("El ClientId no puede estar vacío.");

        if (newDateRange is null)
            throw new InvalidBookingStateException("El DateRange no puede ser nulo.");

        RoomId = roomId;
        ClientId = clientId;
        DateRange = newDateRange;
        Comments = comments;
    }

}