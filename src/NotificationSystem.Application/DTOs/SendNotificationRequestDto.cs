using NotificationSystem.Domain.Enums;

namespace NotificationSystem.Application.DTOs;

public record SendNotificationRequestDto(
    NotificationChannel Channel,
    string Recipient,
    string Content,
    string? Subject = null);
