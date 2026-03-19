using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notifications.Application.Events;
using Notifications.Application.Interfaces;

namespace Notifications.Api.Controllers;

[ApiController]
[Route("api/notifications")]
[AllowAnonymous]
public class UserEventsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public UserEventsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Recebe evento de criação de usuário e simula envio de e-mail de boas-vindas.
    /// </summary>
    [HttpPost("user-created")]
    public async Task<IActionResult> UserCreated(
        [FromBody] UserCreatedEvent userCreatedEvent,
        CancellationToken ct)
    {
        await _notificationService.HandleUserCreatedAsync(userCreatedEvent, ct);
        return NoContent();
    }
}
