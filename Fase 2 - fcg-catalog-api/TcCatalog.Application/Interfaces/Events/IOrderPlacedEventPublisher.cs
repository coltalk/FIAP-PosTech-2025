using TcCatalog.Application.Events;

namespace TcCatalog.Application.Interfaces.Events;

public interface IOrderPlacedEventPublisher
{
    Task PublishAsync(OrderPlacedEvent orderPlacedEvent, CancellationToken ct);
}
