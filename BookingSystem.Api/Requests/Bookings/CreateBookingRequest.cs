namespace BookingSystem.Api.Requests.Bookings;

public record CreateBookingRequest(
    Guid RoomId,
    //Guid ClientId, Lo he eliminado porqué el ClientId se obtiene del CurrentUserService, no es necesario que el cliente lo envíe en la solicitud
    DateTime Start,
    DateTime End,
    string? Comments
);
