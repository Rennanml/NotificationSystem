using Microsoft.Extensions.Logging;
using NotificationSystem.Application.Strategies;
using NotificationSystem.Domain.Entities;
using NotificationSystem.Domain.Enums;

namespace NotificationSystem.Infrastructure.Strategies;

public class EmailNotificationStrategy : INotificationStrategy
{
    private readonly ILogger<EmailNotificationStrategy> _logger;

    public NotificationChannel Channel => NotificationChannel.Email;

    public EmailNotificationStrategy(ILogger<EmailNotificationStrategy> logger)
    {
        _logger = logger;
    }

    public async Task<NotificationResult> SendAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "[EmailStrategy] Iniciando envio de e-mail para '{Recipient}' com assunto '{Subject}'. CorrelationId: {CorrelationId}",
                notification.Recipient,
                notification.Subject ?? "(Sem assunto)",
                notification.CorrelationId);

            // Simulação de latência de rede/chamada de API (SMTP ou SendGrid/Resend)
            await Task.Delay(100, cancellationToken);

            var externalMessageId = $"email_{Guid.NewGuid():N}";

            _logger.LogInformation(
                "[EmailStrategy] E-mail enviado com sucesso para '{Recipient}'. ExternalId: {ExternalId}",
                notification.Recipient,
                externalMessageId);

            return NotificationResult.Success(externalMessageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "[EmailStrategy] Falha ao enviar e-mail para '{Recipient}'. CorrelationId: {CorrelationId}",
                notification.Recipient,
                notification.CorrelationId);

            return NotificationResult.Failure(ex.Message);
        }
    }
}
