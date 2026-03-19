using TcCatalog.Application.Events;
using TcCatalog.Application.Interfaces.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TcCatalog.Api.Controllers;

[ApiController]
[Route("api/payment-events")]
[AllowAnonymous]
public class PaymentEventsController : ControllerBase
{
    private readonly IPaymentProcessedEventConsumer _paymentProcessedEventConsumer;

    public PaymentEventsController(IPaymentProcessedEventConsumer paymentProcessedEventConsumer)
    {
        _paymentProcessedEventConsumer = paymentProcessedEventConsumer;
    }

    [HttpPost("processed")]
    public async Task<IActionResult> Processed([FromBody] PaymentProcessedEvent paymentProcessedEvent, CancellationToken ct)
    {
        await _paymentProcessedEventConsumer.ConsumeAsync(paymentProcessedEvent, ct);
        return NoContent();
    }
}
