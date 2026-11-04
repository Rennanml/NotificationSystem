using NotificationSystem.Domain.Entities;
using NotificationSystem.Domain.Enums;

namespace NotificationSystem.Application.Strategies;

/// <summary>
/// Contrato da Strategy para envio de notificação por canal específico.
/// </summary>
public interface INotificationStrategy
{
    NotificationChannel Channel { get; }

    Task<NotificationResult> SendAsync(Notification notification, CancellationToken cancellationToken = default);
}
