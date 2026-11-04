using NotificationSystem.Domain.Entities;
using NotificationSystem.Domain.Enums;

namespace NotificationSystem.Application.DTOs;

public record NotificationResponseDto(
    Guid NotificationId,
    Guid CorrelationId,
    NotificationChannel Channel,
    string Recipient,
    NotificationStatus Status,
    string? Subject,
    string Content,
    int Attempts,
    string? ErrorMessage,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ProcessedAt)
{
    public static NotificationResponseDto FromEntity(Notification notification) =>
        new(
            notification.Id,
            notification.CorrelationId,
            notification.Channel,
            notification.Recipient,
            notification.Status,
            notification.Subject,
            notification.Content,
            notification.Attempts,
            notification.ErrorMessage,
            notification.CreatedAt,
            notification.ProcessedAt);
}
