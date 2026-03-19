namespace TcCatalog.Application.Events;

public sealed record PaymentProcessedEvent(Guid UserId, Guid JogoId, Guid PayId, int Status);
