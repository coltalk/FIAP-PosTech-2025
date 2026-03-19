using TcCatalog.Application.DTOs;
using TcCatalog.Application.Events;
using TcCatalog.Application.Interfaces;
using TcCatalog.Application.Interfaces.Events;
using TcCatalog.Domain.Entities;
using TcCatalog.Domain.Enums;

namespace TcCatalog.Application.Services;

public class BibliotecaJogoService : IBibliotecaJogoService
{
    private readonly IBibliotecaJogoRepository _bibliotecaRepo;
    private readonly IJogoRepository _jogoRepo;
    private readonly IOrderPlacedEventPublisher _orderPlacedEventPublisher;

    public BibliotecaJogoService(
        IBibliotecaJogoRepository bibliotecaRepo,
        IJogoRepository jogoRepo,
        IOrderPlacedEventPublisher orderPlacedEventPublisher)
    {
        _bibliotecaRepo = bibliotecaRepo;
        _jogoRepo = jogoRepo;
        _orderPlacedEventPublisher = orderPlacedEventPublisher;
    }

    public async Task AddJogoAoUsuarioAsync(Guid userId, Guid jogoId, CancellationToken ct)
    {
        var jogo = await _jogoRepo.GetByIdAsync(jogoId, ct);
        if (jogo is null) throw new KeyNotFoundException("Jogo não encontrado.");

        if (await _bibliotecaRepo.ExistsAsync(userId, jogoId, ct))
            throw new InvalidOperationException("Usuário já possui este jogo na biblioteca.");

        var item = new BibliotecaJogo
        {
            UserId = userId,
            JogoId = jogoId
        };

        await _bibliotecaRepo.AddAsync(item, ct);

        var orderPlacedEvent = new OrderPlacedEvent(userId, jogoId, jogo.Preco);
        await _orderPlacedEventPublisher.PublishAsync(orderPlacedEvent, ct);
    }

    public Task<bool> RemoveJogoDoUsuarioAsync(Guid userId, Guid jogoId, CancellationToken ct)
        => _bibliotecaRepo.RemoveAsync(userId, jogoId, ct);

    public Task<int> RemoveTodosJogosDoUsuarioAsync(Guid userId, CancellationToken ct)
        => _bibliotecaRepo.RemoveAllAsync(userId, ct);


    public async Task ProcessPaymentProcessedEventAsync(PaymentProcessedEvent paymentProcessedEvent, CancellationToken ct)
    {
        var status = paymentProcessedEvent.Status switch
        {
            (int)StatusBibliotecaJogo.Aprovado => StatusBibliotecaJogo.Aprovado,
            (int)StatusBibliotecaJogo.Reprovado => StatusBibliotecaJogo.Reprovado,
            _ => throw new ArgumentException("Status de pagamento inválido. Apenas Aprovado (2) ou Reprovado (3) são permitidos.")
        };

        var updated = await _bibliotecaRepo.UpdateStatusAsync(
            paymentProcessedEvent.UserId,
            paymentProcessedEvent.JogoId,
            paymentProcessedEvent.PayId,
            status,
            ct);

        if (!updated)
            throw new KeyNotFoundException("Registro da biblioteca não encontrado para atualizar status de pagamento.");
    }

    public async Task<IReadOnlyList<BibliotecaJogoDto>> GetJogosDoUsuarioAsync(Guid userId, CancellationToken ct)
    {
        var itensBiblioteca = await _bibliotecaRepo.GetJogosDoUsuarioAsync(userId, ct);

        return itensBiblioteca.Select(item => new BibliotecaJogoDto
        {
            Id = item.Jogo.Id,
            CreatedAtUtc = item.Jogo.CreatedAtUtc,
            UpdatedAtUtc = item.Jogo.UpdatedAtUtc,
            Descricao = item.Jogo.Descricao,
            Genero = item.Jogo.Genero,
            Preco = item.Jogo.Preco,
            DataCadastro = item.Jogo.DataCadastro,
            Status = (int)item.Status,
            StatusDescricao = GetStatusDescricao(item.Status),
            UserId = item.UserId,
            JogoId = item.JogoId,
            PayId = item.PayId
        }).ToList();
    }

    public async Task<IReadOnlyList<MinhaBibliotecaJogoDto>> GetJogosAprovadosDoUsuarioAsync(Guid userId, CancellationToken ct)
    {
        var itensBiblioteca = await _bibliotecaRepo.GetJogosDoUsuarioAsync(userId, ct);

        return itensBiblioteca
            .Where(item => item.Status == StatusBibliotecaJogo.Aprovado)
            .Select(item => new MinhaBibliotecaJogoDto
            {
                Descricao = item.Jogo.Descricao,
                Genero = item.Jogo.Genero,
                Preco = item.Jogo.Preco,
                DataCadastro = item.Jogo.DataCadastro
            })
            .ToList();
    }

    private static string GetStatusDescricao(StatusBibliotecaJogo status)
        => status switch
        {
            StatusBibliotecaJogo.EmAberto => "Pagamento em aberto",
            StatusBibliotecaJogo.Pendente => "Pagamento pendente",
            StatusBibliotecaJogo.Aprovado => "Pagamento APROVADO",
            StatusBibliotecaJogo.Reprovado => "Pagamento REPROVADO",
            _ => "Status desconhecido"
        };
}
