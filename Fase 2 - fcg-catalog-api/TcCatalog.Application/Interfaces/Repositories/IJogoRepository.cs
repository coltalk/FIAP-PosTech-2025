using TcCatalog.Domain.Entities;

namespace TcCatalog.Application.Interfaces;

public interface IJogoRepository
{
    Task AddAsync(Jogo jogo, CancellationToken ct);
    Task<Jogo?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Jogo>> GetAllAsync(CancellationToken ct);
    Task UpdateAsync(Jogo jogo, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task<bool> ExistsByDescricaoGeneroAsync(string descricao, string genero, Guid? ignoreId, CancellationToken ct);
}