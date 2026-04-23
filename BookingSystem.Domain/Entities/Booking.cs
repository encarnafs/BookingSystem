using BookingSystem.Domain.Enums;
using BookingSystem.Domain.Events;
using BookingSystem.Domain.ValueObjects;
using BookingSystem.Domain.Abstractions;

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
            throw new ArgumentException("El RoomId NO puede estar vacío");

        if (clientId == Guid.Empty)
            throw new ArgumentException("El ClientId NO puede estar vacío");

        if (createdByUserId == Guid.Empty)
            throw new ArgumentException("El CreatedByUserId NO puede estar vacío");

        if (dateRange is null)
            throw new ArgumentException("El DateRange NO puede estar vacío");

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
            throw new InvalidOperationException("No se puede modificar una reserva cancelada");

        if (DateRange.End < DateTime.UtcNow)
            throw new InvalidOperationException("No se puede modificar una reserva que ya ha finalizado");

        DateRange = newDateRange;
    }

    public void UpdateComments(string? comments)
    {
        Comments = comments;
    }

    public void Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("La reserva ya está cancelada");

        Status = BookingStatus.Cancelled;
    }

    public void Confirm()
    {
        if (Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("No se puede confirmar una reserva cancelada");

        Status = BookingStatus.Confirmed;
    }

    public void Update(Guid roomId, Guid clientId, DateRange newDateRange, string? comments)
    {
        if (Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("No se puede modificar una reserva cancelada");

        if (DateRange.End < DateTime.UtcNow)
            throw new InvalidOperationException("No se puede modificar una reserva que ya ha finalizado");

        if (roomId == Guid.Empty)
            throw new ArgumentException("El RoomId NO puede estar vacío");

        if (clientId == Guid.Empty)
            throw new ArgumentException("El ClientId NO puede estar vacío");

        if (newDateRange is null)
            throw new ArgumentException("El NewDateRange NO puede estar vacío");  

        RoomId = roomId;
        ClientId = clientId;
        DateRange = newDateRange;
        Comments = comments;
    }

}