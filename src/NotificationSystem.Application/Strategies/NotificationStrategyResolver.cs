using NotificationSystem.Domain.Enums;

namespace NotificationSystem.Application.Strategies;

/// <summary>
/// Implementação concreta do Resolver de Strategies que seleciona em O(1)
/// a estratégia correspondente ao NotificationChannel informado.
/// </summary>
public class NotificationStrategyResolver : INotificationStrategyResolver
{
    private readonly IReadOnlyDictionary<NotificationChannel, INotificationStrategy> _strategies;

    public NotificationStrategyResolver(IEnumerable<INotificationStrategy> strategies)
    {
        ArgumentNullException.ThrowIfNull(strategies);

        var dictionary = new Dictionary<NotificationChannel, INotificationStrategy>();

        foreach (var strategy in strategies)
        {
            if (!dictionary.TryAdd(strategy.Channel, strategy))
            {
                throw new InvalidOperationException(
                    $"Já existe uma estratégia registrada para o canal '{strategy.Channel}'.");
            }
        }

        _strategies = dictionary;
    }

    public INotificationStrategy GetStrategy(NotificationChannel channel)
    {
        if (_strategies.TryGetValue(channel, out var strategy))
        {
            return strategy;
        }

        throw new NotSupportedException(
            $"Nenhuma estratégia de envio encontrada para o canal '{channel}'. Canais suportados no momento: {string.Join(", ", _strategies.Keys)}.");
    }
}
