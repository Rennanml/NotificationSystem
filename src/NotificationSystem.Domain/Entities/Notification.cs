using NotificationSystem.Domain.Enums;

namespace NotificationSystem.Domain.Entities;

public class Notification
{
    public Guid Id { get; private set; }
    public Guid CorrelationId { get; private set; }
    public NotificationChannel Channel { get; private set; }
    public string Recipient { get; private set; } = string.Empty;
    public string? Subject { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public NotificationStatus Status { get; private set; }
    public int Attempts { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? ProcessedAt { get; private set; }

    // EF Core Constructor
    protected Notification() { }

    public Notification(
        Guid correlationId,
        NotificationChannel channel,
        string recipient,
        string content,
        string? subject = null)
    {
        Id = Guid.NewGuid();
        CorrelationId = correlationId == Guid.Empty ? Guid.NewGuid() : correlationId;
        Channel = channel;
        Recipient = recipient?.Trim() ?? throw new ArgumentNullException(nameof(recipient));
        Content = content?.Trim() ?? throw new ArgumentNullException(nameof(content));
        Subject = subject?.Trim();
        Status = NotificationStatus.Queued;
        Attempts = 0;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkAsProcessing()
    {
        Status = NotificationStatus.Processing;
        Attempts++;
        ProcessedAt = DateTimeOffset.UtcNow;
    }

    public void MarkAsDelivered()
    {
        Status = NotificationStatus.Delivered;
        ErrorMessage = null;
        ProcessedAt = DateTimeOffset.UtcNow;
    }

    public void MarkAsFailed(string error)
    {
        Status = NotificationStatus.Failed;
        ErrorMessage = error;
        ProcessedAt = DateTimeOffset.UtcNow;
    }
}
