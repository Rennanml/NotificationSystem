namespace NotificationSystem.Application.Strategies;

public record NotificationResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public string? ExternalMessageId { get; init; }

    public static NotificationResult Success(string? externalMessageId = null) =>
        new() { IsSuccess = true, ExternalMessageId = externalMessageId };

    public static NotificationResult Failure(string errorMessage) =>
        new() { IsSuccess = false, ErrorMessage = errorMessage };
}
