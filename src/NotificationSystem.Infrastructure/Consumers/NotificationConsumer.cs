using MassTransit;
using Microsoft.Extensions.Logging;
using NotificationSystem.Application.Interfaces;
using NotificationSystem.Application.Strategies;
using NotificationSystem.Domain.Entities;
using NotificationSystem.Domain.Enums;
using NotificationSystem.Domain.Events;

namespace NotificationSystem.Infrastructure.Consumers;

public class NotificationConsumer : IConsumer<NotificationRequestedEvent>
{
    private readonly INotificationStrategyResolver _strategyResolver;
    private readonly INotificationRepository _repository;
    private readonly ILogger<NotificationConsumer> _logger;

    public NotificationConsumer(
        INotificationStrategyResolver strategyResolver,
        INotificationRepository repository,
        ILogger<NotificationConsumer> logger)
    {
        _strategyResolver = strategyResolver;
        _repository = repository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<NotificationRequestedEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "[NotificationConsumer] Processando evento de notificação. CorrelationId: {CorrelationId}, Canal: {Channel}, Destinatário: {Recipient}",
            message.CorrelationId,
            message.Channel,
            message.Recipient);

        // 1. Busca notificação no repositório
        var notification = await _repository.GetByCorrelationIdAsync(message.CorrelationId, context.CancellationToken);

        if (notification == null)
        {
            _logger.LogWarning(
                "[NotificationConsumer] Notificação {CorrelationId} não encontrada no banco. Criando registro para auditoria.",
                message.CorrelationId);

            notification = new Notification(
                message.CorrelationId,
                message.Channel,
                message.Recipient,
                message.Content,
                message.Subject);

            await _repository.AddAsync(notification, context.CancellationToken);
        }

        // 2. Verificação de Idempotência
        if (notification.Status == NotificationStatus.Delivered)
        {
            _logger.LogWarning(
                "[NotificationConsumer] Notificação {CorrelationId} já consta como 'Delivered'. Abortando processamento duplicado.",
                message.CorrelationId);
            return;
        }

        // 3. Atualiza status para Processando
        notification.MarkAsProcessing();
        await _repository.UpdateAsync(notification, context.CancellationToken);

        // 4. Executa o Strategy Pattern para o canal solicitado
        try
        {
            var strategy = _strategyResolver.GetStrategy(message.Channel);
            var result = await strategy.SendAsync(notification, context.CancellationToken);

            if (result.IsSuccess)
            {
                notification.MarkAsDelivered();
                await _repository.UpdateAsync(notification, context.CancellationToken);

                _logger.LogInformation(
                    "[NotificationConsumer] Notificação {CorrelationId} enviada com sucesso pelo canal {Channel}. ExternalId: {ExternalId}",
                    message.CorrelationId,
                    message.Channel,
                    result.ExternalMessageId);
            }
            else
            {
                notification.MarkAsFailed(result.ErrorMessage ?? "Erro desconhecido ao enviar notificação.");
                await _repository.UpdateAsync(notification, context.CancellationToken);

                _logger.LogError(
                    "[NotificationConsumer] Falha ao enviar notificação {CorrelationId} pelo canal {Channel}: {Error}",
                    message.CorrelationId,
                    message.Channel,
                    result.ErrorMessage);

                // Dispara exceção para ativar a política de Retry e DLQ do MassTransit
                throw new InvalidOperationException($"Falha no envio da notificação: {result.ErrorMessage}");
            }
        }
        catch (NotSupportedException ex)
        {
            notification.MarkAsFailed(ex.Message);
            await _repository.UpdateAsync(notification, context.CancellationToken);
            _logger.LogCritical(ex, "[NotificationConsumer] Canal não suportado: {Channel}", message.Channel);
            // Canais não suportados não devem sofrer retry contínuo
        }
    }
}
