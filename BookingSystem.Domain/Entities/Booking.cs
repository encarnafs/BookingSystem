using BookingSystem.Domain.Enums;
using BookingSystem.Domain.ValueObjects;

namespace BookingSystem.Domain.Entities;

public class Booking
{
    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateRange DateRange { get; private set; } = default!;
    public BookingStatus Status { get; private set; }
    public string? Comments { get; private set; }
    public DateTime CreatedAt { get; private set; }

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

        Id = Guid.NewGuid();
        RoomId = roomId;
        ClientId = clientId;
        CreatedByUserId = createdByUserId;
        DateRange = dateRange;
        Comments = comments;
        Status = BookingStatus.Pending;
        CreatedAt = DateTime.UtcNow;
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
}