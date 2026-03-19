using Microsoft.Extensions.Logging;
using Notifications.Application.Events;
using Notifications.Application.Interfaces;

namespace Notifications.Application.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public Task HandleUserCreatedAsync(UserCreatedEvent evt, CancellationToken ct)
    {
        _logger.LogInformation(
            "[EMAIL SIMULADO] Bem-vindo ao FCG! | Para: {Email} | Nome: {Name} | UserId: {UserId}",
            evt.Email, evt.Name, evt.UserId);

        return Task.CompletedTask;
    }

    public Task HandlePaymentProcessedAsync(PaymentProcessedEvent evt, CancellationToken ct)
    {
        switch (evt.Status)
        {
            case 2: // Aprovado
                _logger.LogInformation(
                    "[EMAIL SIMULADO] Pagamento aprovado! | UserId: {UserId} | JogoId: {JogoId} | PayId: {PayId}",
                    evt.UserId, evt.JogoId, evt.PayId);
                break;

            case 3: // Reprovado
                _logger.LogWarning(
                    "[EMAIL SIMULADO] Pagamento reprovado. | UserId: {UserId} | JogoId: {JogoId} | PayId: {PayId}",
                    evt.UserId, evt.JogoId, evt.PayId);
                break;

            default:
                _logger.LogInformation(
                    "Evento de pagamento recebido com status {Status} | UserId: {UserId} | JogoId: {JogoId} | PayId: {PayId}",
                    evt.Status, evt.UserId, evt.JogoId, evt.PayId);
                break;
        }

        return Task.CompletedTask;
    }
}
