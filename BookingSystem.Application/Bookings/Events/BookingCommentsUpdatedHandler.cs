using MediatR;
using Microsoft.Extensions.Logging;
using BookingSystem.Application.Common.Interfaces;

namespace BookingSystem.Application.Bookings.Events;

public class BookingCommentsUpdatedHandler : INotificationHandler<BookingCommentsUpdatedNotification>
{
    private readonly ILogger<BookingCommentsUpdatedHandler> _logger;
    private readonly IEmailService _emailService;

    public BookingCommentsUpdatedHandler(
        ILogger<BookingCommentsUpdatedHandler> logger,
        IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Handle(BookingCommentsUpdatedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Procesando actualización de comentarios de reserva {BookingId}", notification.BookingId);

        await _emailService.SendAsync(
            to: notification.Email,
            subject: "Comentarios de reserva actualizados",
            body: $"Los comentarios de la reserva con ID {notification.BookingId} han sido actualizados.");

        _logger.LogInformation("Email enviado por actualización de comentarios {BookingId}", notification.BookingId);
    }
}
