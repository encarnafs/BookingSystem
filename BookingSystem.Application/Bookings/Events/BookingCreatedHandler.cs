using MediatR;
using Microsoft.Extensions.Logging;
using BookingSystem.Application.Common.Interfaces;

namespace BookingSystem.Application.Bookings.Events;

public class BookingCreatedHandler : INotificationHandler<BookingCreatedNotification>
{
    private readonly ILogger<BookingCreatedHandler> _logger;
    private readonly IEmailService _emailService;

    public BookingCreatedHandler(
        ILogger<BookingCreatedHandler> logger,
        IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Handle(BookingCreatedNotification notification, CancellationToken cancellationToken)
    {
        // 1. Log de inicio del handler
        _logger.LogInformation("Reserva creada con ID: {BookingId}", notification.BookingId);

        // 2. Log de inicio del envío del email
        _logger.LogInformation("Iniciando envío de email por creación de reserva {BookingId}", notification.BookingId);

        // 3. Envío del email
        await _emailService.SendAsync(
            to: "encarnifs@gmail.com",
            subject: "Nueva reserva creada",
            body: $"Se ha creado la reserva con ID {notification.BookingId}");

        // 4. Log de éxito del envío
        _logger.LogInformation("Email enviado por la creación de la reserva {BookingId}", notification.BookingId);

    }
}
