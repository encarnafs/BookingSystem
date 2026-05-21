using BookingSystem.Application.Bookings.Dtos;
using MediatR;

namespace BookingSystem.Application.Bookings.Commands.CreateBooking;

// NOTA IMPORTANTE:
// Este command no puede ser un 'record' posicional porque sus propiedades se generan
// automáticamente como 'init' (solo asignables durante la inicialización del objeto).
// El handler necesita modificar ClientId cuando el usuario autenticado tiene rol 'Client',
// para evitar que un cliente pueda reservar en nombre de otro. Esa reasignación no es
// posible en un record posicional debido a su inmutabilidad.
// Por este motivo, este command debe ser una 'class' con propiedades 'set' para permitir
// que la capa de aplicación ajuste los valores según el rol del usuario.
public class CreateBookingCommand : IRequest<BookingDto>
{
    public Guid RoomId { get; set; }
    public Guid ClientId { get; set; }   // ⭐ Sin record se puede modificar en el handler
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string? Comments { get; set; }
}
