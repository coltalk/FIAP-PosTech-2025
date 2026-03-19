using TcCatalog.Application.Events;

namespace TcCatalog.Application.Interfaces.Events;

public interface IPaymentProcessedEventConsumer
{
    Task ConsumeAsync(PaymentProcessedEvent paymentProcessedEvent, CancellationToken ct);
}
