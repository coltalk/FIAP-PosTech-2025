namespace Notifications.Application.Events;

public sealed record UserCreatedEvent(
    Guid UserId,
    string Name,
    string Email);
