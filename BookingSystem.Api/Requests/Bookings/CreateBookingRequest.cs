namespace BookingSystem.Api.Requests.Bookings;

/// <summary>
/// Datos necesarios para crear una nueva reserva.
/// </summary>
public record CreateBookingRequest(

    Guid RoomId,
    //Guid ClientId, Lo he eliminado porqué el ClientId se obtiene del CurrentUserService, no es necesario que el cliente lo envíe en la solicitud
    Guid? ClientId, //Opcional, para el Admin se enviará y para el Client no porqué ya cogerá el Id cuando se registre y loguee.
    DateTime Start,
    DateTime End,
    string? Comments
);
