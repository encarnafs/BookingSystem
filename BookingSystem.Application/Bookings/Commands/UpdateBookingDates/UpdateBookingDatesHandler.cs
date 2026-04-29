using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.ValueObjects;
using MediatR;

namespace BookingSystem.Application.Bookings.Commands.UpdateBookingDates;

public class UpdateBookingDatesHandler : IRequestHandler<UpdateBookingDatesCommand>
{
    private readonly IBookingRepository _bookingRepository;

    public UpdateBookingDatesHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task Handle(UpdateBookingDatesCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener la reserva
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken)
            ?? throw new NotFoundException("Booking", request.BookingId);

        // 2. Crear el nuevo rango de fechas
        var newDateRange = new DateRange(request.Start, request.End);

        // 3. Validar solapamientos excluyendo la propia reserva
        var hasOverlap = await _bookingRepository.ExistsOverlappingBookingAsync(
            booking.RoomId,
            request.Start,
            request.End,
            booking.Id,
            cancellationToken
        );

        if (hasOverlap)
            throw new ValidationException("Las nuevas fechas se solapan con otra reserva existente.");

        // 4. Actualizar la entidad
        booking.UpdateDates(newDateRange);

        // 5. Guardar cambios
        await _bookingRepository.UpdateAsync(booking, cancellationToken);
    }
}
