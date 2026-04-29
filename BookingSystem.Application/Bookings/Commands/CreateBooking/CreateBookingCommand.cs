using BookingSystem.Application.Bookings.Dtos;
using MediatR;

namespace BookingSystem.Application.Bookings.Commands.CreateBooking;

public record CreateBookingCommand(
    Guid RoomId,
    Guid ClientId,
    //Guid CreatedByUserId, Si dejamos que el cliente envíe el CreatedByUserId, podríamos tener problemas de seguridad (un cliente malintencionado podría enviar el ID de otro usuario). Por eso, es mejor obtenerlo del token de autenticación en el handler.
    DateTime Start,
    DateTime End,
    string? Comments
) : IRequest<BookingDto>;