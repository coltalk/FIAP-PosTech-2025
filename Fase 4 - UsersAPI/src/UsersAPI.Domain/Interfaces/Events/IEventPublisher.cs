using UsersAPI.Domain.Events;

namespace UsersAPI.Domain.Interfaces.Events;

public interface IEventPublisher
{
    Task PublishUserCreatedAsync(UserCreatedEvent userEvent);
}