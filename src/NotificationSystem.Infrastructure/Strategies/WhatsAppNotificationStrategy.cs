using Microsoft.Extensions.Logging;
using NotificationSystem.Application.Strategies;
using NotificationSystem.Domain.Entities;
using NotificationSystem.Domain.Enums;

namespace NotificationSystem.Infrastructure.Strategies;

public class WhatsAppNotificationStrategy : INotificationStrategy
{
    private readonly ILogger<WhatsAppNotificationStrategy> _logger;

    public NotificationChannel Channel => NotificationChannel.WhatsApp;

    public WhatsAppNotificationStrategy(ILogger<WhatsAppNotificationStrategy> logger)
    {
        _logger = logger;
    }

    public async Task<NotificationResult> SendAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "[WhatsAppStrategy] Disparando mensagem WhatsApp para '{Recipient}'. CorrelationId: {CorrelationId}",
                notification.Recipient,
                notification.CorrelationId);

            // Simulação de chamada de API HTTP do WhatsApp (Meta Cloud API / Z-API / Twilio)
            await Task.Delay(120, cancellationToken);

            var wamid = $"wamid_{Guid.NewGuid():N}";

            _logger.LogInformation(
                "[WhatsAppStrategy] Mensagem WhatsApp entregue com sucesso para '{Recipient}'. WAMID: {Wamid}",
                notification.Recipient,
                wamid);

            return NotificationResult.Success(wamid);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "[WhatsAppStrategy] Erro ao enviar mensagem WhatsApp para '{Recipient}'. CorrelationId: {CorrelationId}",
                notification.Recipient,
                notification.CorrelationId);

            return NotificationResult.Failure(ex.Message);
        }
    }
}
