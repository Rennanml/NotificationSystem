using System.Text.RegularExpressions;
using FluentValidation;
using NotificationSystem.Application.DTOs;
using NotificationSystem.Domain.Enums;

namespace NotificationSystem.Application.Validators;

public partial class SendNotificationRequestValidator : AbstractValidator<SendNotificationRequestDto>
{
    // Formato E.164 opcional com '+', seguido de 8 a 15 dígitos numéricos
    private static readonly Regex PhoneRegex = new(@"^\+?[1-9]\d{7,14}$", RegexOptions.Compiled);

    public SendNotificationRequestValidator()
    {
        RuleFor(x => x.Channel)
            .IsInEnum()
            .WithMessage("Canal de notificação inválido.");

        RuleFor(x => x.Recipient)
            .NotEmpty()
            .WithMessage("O destinatário é obrigatório.");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("A mensagem não pode ser vazia.")
            .MaximumLength(2000)
            .WithMessage("A mensagem não pode ultrapassar 2000 caracteres.");

        // Validação específica por canal:
        When(x => x.Channel == NotificationChannel.Email, () =>
        {
            RuleFor(x => x.Recipient)
                .EmailAddress()
                .WithMessage("O destinatário informado não é um endereço de e-mail válido.");
        });

        When(x => x.Channel == NotificationChannel.WhatsApp || x.Channel == NotificationChannel.Sms, () =>
        {
            RuleFor(x => x.Recipient)
                .Must(BeAValidPhoneNumber)
                .WithMessage("O destinatário deve ser um número de telefone válido com DDI e DDD (ex: +5511999998888).");
        });
    }

    private static bool BeAValidPhoneNumber(string recipient)
    {
        if (string.IsNullOrWhiteSpace(recipient)) return false;
        var sanitized = recipient.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
        return PhoneRegex.IsMatch(sanitized);
    }
}
