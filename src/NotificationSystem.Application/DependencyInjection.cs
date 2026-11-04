using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NotificationSystem.Application.Strategies;
using NotificationSystem.Application.Validators;

namespace NotificationSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registra o resolver de strategies como Scoped
        services.AddScoped<INotificationStrategyResolver, NotificationStrategyResolver>();

        // Registra validadores do FluentValidation
        services.AddValidatorsFromAssemblyContaining<SendNotificationRequestValidator>();

        return services;
    }
}
