using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using NotificationSystem.Application.DTOs;
using NotificationSystem.Application.Interfaces;
using NotificationSystem.Domain.Entities;
using NotificationSystem.Domain.Events;

namespace NotificationAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly INotificationRepository _repository;
    private readonly IValidator<SendNotificationRequestDto> _validator;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(
        IPublishEndpoint publishEndpoint,
        INotificationRepository repository,
        IValidator<SendNotificationRequestDto> validator,
        ILogger<NotificationController> logger)
    {
        _publishEndpoint = publishEndpoint;
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    /// <summary>
    /// Enfileira uma nova solicitação de notificação (E-mail, WhatsApp, SMS, etc.).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(NotificationResponseDto), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RequestNotification(
        [FromBody] SendNotificationRequestDto request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        var correlationId = Guid.NewGuid();
        var notification = new Notification(
            correlationId,
            request.Channel,
            request.Recipient,
            request.Content,
            request.Subject);

        // 1. Persiste inicialmente com status 'Queued' no PostgreSQL
        await _repository.AddAsync(notification, cancellationToken);

        // 2. Publica evento na fila do RabbitMQ
        await _publishEndpoint.Publish(new NotificationRequestedEvent
        {
            CorrelationId = notification.CorrelationId,
            NotificationId = notification.Id,
            Channel = notification.Channel,
            Recipient = notification.Recipient,
            Subject = notification.Subject,
            Content = notification.Content,
            CreatedAt = notification.CreatedAt
        }, cancellationToken);

        _logger.LogInformation(
            "Notificação enfileirada com sucesso. CorrelationId: {CorrelationId}, Canal: {Channel}",
            notification.CorrelationId,
            notification.Channel);

        return AcceptedAtAction(
            nameof(GetStatus),
            new { correlationId = notification.CorrelationId },
            NotificationResponseDto.FromEntity(notification));
    }

    /// <summary>
    /// Consulta o status e histórico de entrega de uma notificação pelo CorrelationId.
    /// </summary>
    [HttpGet("{correlationId:guid}")]
    [ProducesResponseType(typeof(NotificationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatus(Guid correlationId, CancellationToken cancellationToken)
    {
        var notification = await _repository.GetByCorrelationIdAsync(correlationId, cancellationToken);

        if (notification == null)
        {
            return NotFound(new { message = $"Notificação com CorrelationId '{correlationId}' não foi encontrada." });
        }

        return Ok(NotificationResponseDto.FromEntity(notification));
    }
}