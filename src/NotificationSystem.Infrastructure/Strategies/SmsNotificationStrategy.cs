using Microsoft.Extensions.Logging;
using NotificationSystem.Application.Strategies;
using NotificationSystem.Domain.Entities;
using NotificationSystem.Domain.Enums;

namespace NotificationSystem.Infrastructure.Strategies;

public class SmsNotificationStrategy : INotificationStrategy
{
    private readonly ILogger<SmsNotificationStrategy> _logger;

    public NotificationChannel Channel => NotificationChannel.Sms;

    public SmsNotificationStrategy(ILogger<SmsNotificationStrategy> logger)
    {
        _logger = logger;
    }

    public async Task<NotificationResult> SendAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "[SmsStrategy] Disparando SMS para '{Recipient}'. CorrelationId: {CorrelationId}",
                notification.Recipient,
                notification.CorrelationId);

            // Simulação de gateway de SMS (Twilio / AWS SNS / TotalVoice)
            await Task.Delay(80, cancellationToken);

            var smsId = $"sms_{Guid.NewGuid():N}";

            _logger.LogInformation(
                "[SmsStrategy] SMS enviado com sucesso para '{Recipient}'. MessageId: {MessageId}",
                notification.Recipient,
                smsId);

            return NotificationResult.Success(smsId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "[SmsStrategy] Erro ao enviar SMS para '{Recipient}'. CorrelationId: {CorrelationId}",
                notification.Recipient,
                notification.CorrelationId);

            return NotificationResult.Failure(ex.Message);
        }
    }
}
