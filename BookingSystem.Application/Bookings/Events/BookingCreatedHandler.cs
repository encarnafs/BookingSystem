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
        _logger.LogInformation("Reserva creada con ID: {BookingId}", notification.BookingId);

        await _emailService.SendAsync(
            to: notification.Email,
            subject: "Nueva reserva creada",
            body: $"Tu reserva con ID {notification.BookingId} ha sido creada correctamente.");

        _logger.LogInformation("Email enviado por la creación de la reserva {BookingId}", notification.BookingId);
    }
}
