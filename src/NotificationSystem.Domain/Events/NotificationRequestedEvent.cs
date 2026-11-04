using NotificationSystem.Domain.Enums;

namespace NotificationSystem.Domain.Events;

public record NotificationRequestedEvent
{
    public Guid CorrelationId { get; init; }
    public Guid NotificationId { get; init; }
    public NotificationChannel Channel { get; init; }
    public string Recipient { get; init; } = string.Empty;
    public string? Subject { get; init; }
    public string Content { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}
