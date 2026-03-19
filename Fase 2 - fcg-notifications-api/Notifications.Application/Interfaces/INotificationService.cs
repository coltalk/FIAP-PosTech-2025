using Notifications.Application.Events;

namespace Notifications.Application.Interfaces;

public interface INotificationService
{
    Task HandleUserCreatedAsync(UserCreatedEvent evt, CancellationToken ct);
    Task HandlePaymentProcessedAsync(PaymentProcessedEvent evt, CancellationToken ct);
}
