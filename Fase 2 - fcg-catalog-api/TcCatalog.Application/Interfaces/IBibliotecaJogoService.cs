using TcCatalog.Application.DTOs;
using TcCatalog.Application.Events;

namespace TcCatalog.Application.Interfaces;

public interface IBibliotecaJogoService
{
    Task AddJogoAoUsuarioAsync(Guid userId, Guid jogoId, CancellationToken ct);
    Task<bool> RemoveJogoDoUsuarioAsync(Guid userId, Guid jogoId, CancellationToken ct);
    Task<int> RemoveTodosJogosDoUsuarioAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<BibliotecaJogoDto>> GetJogosDoUsuarioAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<MinhaBibliotecaJogoDto>> GetJogosAprovadosDoUsuarioAsync(Guid userId, CancellationToken ct);
    Task ProcessPaymentProcessedEventAsync(PaymentProcessedEvent paymentProcessedEvent, CancellationToken ct);
}
