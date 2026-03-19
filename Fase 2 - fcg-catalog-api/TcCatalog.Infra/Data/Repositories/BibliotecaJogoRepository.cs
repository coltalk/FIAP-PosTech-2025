using Microsoft.EntityFrameworkCore;
using TcCatalog.Application.Interfaces;
using TcCatalog.Domain.Entities;
using TcCatalog.Domain.Enums;
using TcCatalog.Infra.Data.Context.Sql.Context;

namespace TcCatalog.Infra.Data.Repositories;

public class BibliotecaJogoRepository : IBibliotecaJogoRepository
{
    private readonly TcCatalogDbContext _db;

    public BibliotecaJogoRepository(TcCatalogDbContext db) => _db = db;

    public Task<bool> ExistsAsync(Guid userId, Guid jogoId, CancellationToken ct)
        => _db.BibliotecaJogos.AnyAsync(x => x.UserId == userId && x.JogoId == jogoId, ct);

    public async Task AddAsync(BibliotecaJogo item, CancellationToken ct)
    {
        _db.BibliotecaJogos.Add(item);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<bool> RemoveAsync(Guid userId, Guid jogoId, CancellationToken ct)
    {
        var item = await _db.BibliotecaJogos
            .FirstOrDefaultAsync(x => x.UserId == userId && x.JogoId == jogoId, ct);

        if (item is null) return false;

        _db.BibliotecaJogos.Remove(item);
        await _db.SaveChangesAsync(ct);
        return true;
    }


    public async Task<int> RemoveAllAsync(Guid userId, CancellationToken ct)
    {
        var itens = await _db.BibliotecaJogos
            .Where(x => x.UserId == userId)
            .ToListAsync(ct);

        if (itens.Count == 0) return 0;

        _db.BibliotecaJogos.RemoveRange(itens);
        await _db.SaveChangesAsync(ct);
        return itens.Count;
    }

    public async Task<IReadOnlyList<BibliotecaJogo>> GetJogosDoUsuarioAsync(Guid userId, CancellationToken ct)
    {
        return await _db.BibliotecaJogos
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Include(x => x.Jogo)
            .OrderBy(x => x.Jogo.Descricao)
            .ToListAsync(ct);
    }

    public async Task<bool> UpdateStatusAsync(Guid userId, Guid jogoId, Guid payId, StatusBibliotecaJogo status, CancellationToken ct)
    {
        var item = await _db.BibliotecaJogos
            .FirstOrDefaultAsync(x => x.UserId == userId && x.JogoId == jogoId, ct);

        if (item is null) return false;

        item.PayId = payId;
        item.Status = status;
        item.Touch();

        await _db.SaveChangesAsync(ct);
        return true;
    }
}
