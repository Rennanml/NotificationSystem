using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationSystem.Application.Interfaces;
using NotificationSystem.Application.Strategies;
using NotificationSystem.Infrastructure.Persistence;
using NotificationSystem.Infrastructure.Persistence.Repositories;
using NotificationSystem.Infrastructure.Strategies;

namespace NotificationSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. Configuração do EF Core com PostgreSQL
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=notificationdb;Username=postgres;Password=password123";

        services.AddDbContext<NotificationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // 2. Registro do repositório
        services.AddScoped<INotificationRepository, NotificationRepository>();

        // 3. Registro das estratégias concretas do Strategy Pattern
        services.AddScoped<INotificationStrategy, EmailNotificationStrategy>();
        services.AddScoped<INotificationStrategy, WhatsAppNotificationStrategy>();
        services.AddScoped<INotificationStrategy, SmsNotificationStrategy>();

        return services;
    }
}
