using TcUser.Application.Events;

namespace TcUser.Application.Interfaces;

public interface IUserCreatedEventPublisher
{
    Task PublishAsync(UserCreatedEvent message, CancellationToken ct);
}
