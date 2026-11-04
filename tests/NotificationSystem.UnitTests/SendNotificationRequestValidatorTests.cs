using NotificationSystem.Application.DTOs;
using NotificationSystem.Application.Validators;
using NotificationSystem.Domain.Enums;
using Xunit;

namespace NotificationSystem.UnitTests;

public class SendNotificationRequestValidatorTests
{
    private readonly SendNotificationRequestValidator _validator = new();

    [Fact]
    public async Task Validate_WhenEmailIsValid_ShouldPass()
    {
        var request = new SendNotificationRequestDto(
            Channel: NotificationChannel.Email,
            Recipient: "usuario@teste.com",
            Content: "Sua mensagem foi enviada",
            Subject: "Assunto do Email");

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_WhenEmailIsInvalid_ShouldFail()
    {
        var request = new SendNotificationRequestDto(
            Channel: NotificationChannel.Email,
            Recipient: "email-invalido-sem-arroba",
            Content: "Mensagem de teste");

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(request.Recipient));
    }

    [Fact]
    public async Task Validate_WhenWhatsAppPhoneNumberIsValid_ShouldPass()
    {
        var request = new SendNotificationRequestDto(
            Channel: NotificationChannel.WhatsApp,
            Recipient: "+5511999998888",
            Content: "Mensagem via WhatsApp");

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_WhenWhatsAppPhoneNumberIsInvalid_ShouldFail()
    {
        var request = new SendNotificationRequestDto(
            Channel: NotificationChannel.WhatsApp,
            Recipient: "telefone-letras-123",
            Content: "Mensagem via WhatsApp");

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(request.Recipient));
    }

    [Fact]
    public async Task Validate_WhenContentIsEmpty_ShouldFail()
    {
        var request = new SendNotificationRequestDto(
            Channel: NotificationChannel.Email,
            Recipient: "teste@dominio.com",
            Content: "");

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(request.Content));
    }
}
