using Microsoft.Extensions.Logging.Abstractions;
using NotificationSystem.Application.Strategies;
using NotificationSystem.Domain.Enums;
using NotificationSystem.Infrastructure.Strategies;
using Xunit;

namespace NotificationSystem.UnitTests;

public class NotificationStrategyResolverTests
{
    private readonly List<INotificationStrategy> _strategies;

    public NotificationStrategyResolverTests()
    {
        _strategies = new List<INotificationStrategy>
        {
            new EmailNotificationStrategy(NullLogger<EmailNotificationStrategy>.Instance),
            new WhatsAppNotificationStrategy(NullLogger<WhatsAppNotificationStrategy>.Instance),
            new SmsNotificationStrategy(NullLogger<SmsNotificationStrategy>.Instance)
        };
    }

    [Theory]
    [InlineData(NotificationChannel.Email, typeof(EmailNotificationStrategy))]
    [InlineData(NotificationChannel.WhatsApp, typeof(WhatsAppNotificationStrategy))]
    [InlineData(NotificationChannel.Sms, typeof(SmsNotificationStrategy))]
    public void GetStrategy_WhenChannelIsRegistered_ShouldReturnCorrectStrategy(
        NotificationChannel channel,
        Type expectedType)
    {
        // Arrange
        var resolver = new NotificationStrategyResolver(_strategies);

        // Act
        var strategy = resolver.GetStrategy(channel);

        // Assert
        Assert.NotNull(strategy);
        Assert.IsType(expectedType, strategy);
        Assert.Equal(channel, strategy.Channel);
    }

    [Fact]
    public void GetStrategy_WhenChannelIsNotRegistered_ShouldThrowNotSupportedException()
    {
        // Arrange - resolver apenas com Email
        var onlyEmailStrategy = new List<INotificationStrategy>
        {
            new EmailNotificationStrategy(NullLogger<EmailNotificationStrategy>.Instance)
        };
        var resolver = new NotificationStrategyResolver(onlyEmailStrategy);

        // Act & Assert
        var ex = Assert.Throws<NotSupportedException>(() =>
            resolver.GetStrategy(NotificationChannel.WhatsApp));

        Assert.Contains("WhatsApp", ex.Message);
    }

    [Fact]
    public void Constructor_WhenDuplicateStrategiesExistForSameChannel_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var duplicates = new List<INotificationStrategy>
        {
            new EmailNotificationStrategy(NullLogger<EmailNotificationStrategy>.Instance),
            new EmailNotificationStrategy(NullLogger<EmailNotificationStrategy>.Instance)
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => new NotificationStrategyResolver(duplicates));
    }
}
