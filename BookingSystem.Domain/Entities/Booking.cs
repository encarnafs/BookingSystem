using BookingSystem.Domain.Enums;
using BookingSystem.Domain.ValueObjects;
using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Domain.Entities;

public class Booking
{
    // -----------------------------
    // Propiedades
    // -----------------------------
    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateRange DateRange { get; private set; } = default!;
    public BookingStatus Status { get; private set; }
    public string? Comments { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navegación
    public Room Room { get; private set; } = default!;
    public Client Client { get; private set; } = default!;

    // Constructor privado para EF Core
    private Booking() { }

    // -----------------------------
    // Constructor principal
    // -----------------------------
    public Booking(Guid roomId, Guid clientId, Guid createdByUserId, DateRange dateRange, string? comments = null)
    {
        Id = Guid.NewGuid();
        RoomId = ValidateRoomId(roomId);
        ClientId = ValidateClientId(clientId);
        CreatedByUserId = ValidateCreatedByUserId(createdByUserId);
        DateRange = ValidateDateRange(dateRange);

        //No permitir reservas en el pasado
        if (DateRange.Start < DateTime.UtcNow)
            throw new InvalidBookingStateException("No se pueden crear reservas en el pasado.");

        Comments = comments;
        Status = BookingStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    // -----------------------------
    // Métodos públicos
    // -----------------------------
    public void UpdateDates(DateRange newDateRange)
    {
        if (Status == BookingStatus.Cancelled)
            throw new InvalidBookingStateException("Cancelled");

        if (newDateRange.End < DateTime.UtcNow)
            throw new InvalidBookingStateException("PastEndDate");

        DateRange = ValidateDateRange(newDateRange);
    }

    public void UpdateComments(string? comments)
    {
        if (Status == BookingStatus.Cancelled)
            throw new InvalidBookingStateException("Cancelled");

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
            throw new InvalidBookingStateException("Cancelled");

        if (Status == BookingStatus.Confirmed)
            throw new BookingAlreadyConfirmedException(Id);

        Status = BookingStatus.Confirmed;
    }

    public void Update(Guid roomId, Guid clientId, DateRange newDateRange, string? comments)
    {
        if (Status == BookingStatus.Cancelled)
            throw new InvalidBookingStateException("Cancelled");
        
        if (Status == BookingStatus.Confirmed)
            throw new InvalidBookingStateException("Confirmed");

        if (newDateRange.End < DateTime.UtcNow)
            throw new InvalidBookingStateException("PastEndDate");

        RoomId = ValidateRoomId(roomId);
        ClientId = ValidateClientId(clientId);
        DateRange = ValidateDateRange(newDateRange);
        Comments = comments;
    }

    // -----------------------------
    // Métodos privados (helpers)
    // -----------------------------
    private static Guid ValidateRoomId(Guid roomId)
    {
        if (roomId == Guid.Empty)
            throw new InvalidBookingStateException(roomId.ToString());
        return roomId;
    }

    private static Guid ValidateClientId(Guid clientId)
    {
        if (clientId == Guid.Empty)
            throw new InvalidBookingStateException(clientId.ToString());
        return clientId;
    }

    private static Guid ValidateCreatedByUserId(Guid createdByUserId)
    {
        if (createdByUserId == Guid.Empty)
            throw new InvalidBookingStateException(createdByUserId.ToString());
        return createdByUserId;
    }

    private static DateRange ValidateDateRange(DateRange dateRange)
    {
        if (dateRange is null)
            throw new InvalidBookingStateException("null");
        return dateRange;
    }
}

