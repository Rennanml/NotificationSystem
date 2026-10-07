namespace NotificationAPI.Events;

public record NotificationRequestedEvent
{
    public Guid CorrelationId { get; init; }
    public string UserEmail { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}