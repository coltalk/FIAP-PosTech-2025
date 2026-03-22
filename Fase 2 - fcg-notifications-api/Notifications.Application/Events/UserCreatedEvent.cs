namespace Notifications.Application.Events;

public sealed record UserCreatedEvent(
    Guid UserId,
    string Name,
    string Email,
    string? Role,
    DateTime? CreatedAtUtc);
