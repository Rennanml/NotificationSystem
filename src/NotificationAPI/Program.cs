using System.Text.Json.Serialization;
using MassTransit;
using NotificationSystem.Application;
using NotificationSystem.Infrastructure;
using NotificationSystem.Infrastructure.Consumers;
using NotificationSystem.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// 1. Registra camadas da Clean Architecture
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// 2. Configura MassTransit com RabbitMQ e Consumers
builder.Services.AddMassTransit(x =>
{
    // Registra o consumidor assíncrono
    x.AddConsumer<NotificationConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
        var rabbitUser = builder.Configuration["RabbitMQ:Username"] ?? "guest";
        var rabbitPass = builder.Configuration["RabbitMQ:Password"] ?? "guest";

        cfg.Host(rabbitHost, "/", h =>
        {
            h.Username(rabbitUser);
            h.Password(rabbitPass);
        });

        // Configuração de resiliência: Retry com backoff exponencial
        cfg.UseMessageRetry(r => r.Intervals(
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(10)));

        cfg.ConfigureEndpoints(context);
    });
});

// 3. Controllers e serialização de Enums
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 4. Criação e garantia do esquema do banco de dados no PostgreSQL
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
    try
    {
        await dbContext.Database.EnsureCreatedAsync();
        app.Logger.LogInformation("Banco de dados PostgreSQL verificado/inicializado com sucesso.");
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, "Não foi possível conectar ao PostgreSQL no startup. Certifique-se de que o container do banco está ativo.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();