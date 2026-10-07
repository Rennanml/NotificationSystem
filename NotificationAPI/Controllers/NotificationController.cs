using MassTransit;
using Microsoft.AspNetCore.Mvc;
using NotificationAPI.Events;

namespace NotificationAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;

    public NotificationController(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task<IActionResult> RequestNotification([FromBody] NotificationRequestDto request)
    {
        var evento = new NotificationRequestedEvent
        {
            CorrelationId = Guid.NewGuid(),
            UserEmail = request.Email,
            Message = request.Message
        };

        await _publishEndpoint.Publish(evento);

        return Accepted(new { message = "Sua notificação foi enfileirada com sucesso!", correlationId = evento.CorrelationId });
    }
}

public record NotificationRequestDto(string Email, string Message);