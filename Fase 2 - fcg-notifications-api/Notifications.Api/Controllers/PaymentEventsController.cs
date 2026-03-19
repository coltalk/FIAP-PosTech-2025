using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notifications.Application.Events;
using Notifications.Application.Interfaces;

namespace Notifications.Api.Controllers;

[ApiController]
[Route("api/notifications")]
[AllowAnonymous]
public class PaymentEventsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public PaymentEventsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Recebe evento de pagamento processado e simula envio de e-mail de confirmação.
    /// </summary>
    [HttpPost("payment-processed")]
    public async Task<IActionResult> PaymentProcessed(
        [FromBody] PaymentProcessedEvent paymentProcessedEvent,
        CancellationToken ct)
    {
        await _notificationService.HandlePaymentProcessedAsync(paymentProcessedEvent, ct);
        return NoContent();
    }
}
