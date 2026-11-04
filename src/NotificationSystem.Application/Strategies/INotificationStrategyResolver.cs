using NotificationSystem.Domain.Enums;

namespace NotificationSystem.Application.Strategies;

/// <summary>
/// Contrato do Resolver / Factory de Strategies de Notificação.
/// </summary>
public interface INotificationStrategyResolver
{
    INotificationStrategy GetStrategy(NotificationChannel channel);
}
