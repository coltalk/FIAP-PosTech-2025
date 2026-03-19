using TcCatalog.Application.DTOs;
using TcCatalog.Application.Interfaces;
using TcCatalog.Domain.Entities;

namespace TcCatalog.Application.Services;

public class JogoService : IJogoService
{
    private readonly IJogoRepository _repo;

    public JogoService(IJogoRepository repo) => _repo = repo;

    public async Task<JogoDto> CreateAsync(JogoDto dto, CancellationToken ct)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        var descricao = dto.Descricao.Trim();
        var genero = dto.Genero.Trim();

        if (await _repo.ExistsByDescricaoGeneroAsync(descricao, genero, null, ct))
            throw new InvalidOperationException("Já existe jogo com a mesma descrição e gênero.");

        var jogo = new Jogo
        {
            Descricao = descricao,
            Genero = genero,
            Preco = dto.Preco,
            DataCadastro = DateTime.UtcNow
        };

        await _repo.AddAsync(jogo, ct);
        return ToDto(jogo);
    }

    public async Task<JogoDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var jogo = await _repo.GetByIdAsync(id, ct);
        return jogo is null ? null : ToDto(jogo);
    }

    public async Task<IReadOnlyList<JogoDto>> GetAllAsync(CancellationToken ct)
    {
        var jogos = await _repo.GetAllAsync(ct);
        return jogos.Select(ToDto).ToList();
    }

    public async Task<JogoDto?> UpdateAsync(Guid id, JogoDto dto, CancellationToken ct)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        var jogo = await _repo.GetByIdAsync(id, ct);
        if (jogo is null) return null;

        var descricao = dto.Descricao.Trim();
        var genero = dto.Genero.Trim();

        if (await _repo.ExistsByDescricaoGeneroAsync(descricao, genero, id, ct))
            throw new InvalidOperationException("Já existe jogo com a mesma descrição e gênero.");

        jogo.Descricao = descricao;
        jogo.Genero = genero;
        jogo.Preco = dto.Preco;
        jogo.Touch();

        await _repo.UpdateAsync(jogo, ct);
        return ToDto(jogo);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct)
        => _repo.DeleteAsync(id, ct);

    private static JogoDto ToDto(Jogo jogo) => new()
    {
        Id = jogo.Id,
        CreatedAtUtc = jogo.CreatedAtUtc,
        UpdatedAtUtc = jogo.UpdatedAtUtc,
        Descricao = jogo.Descricao,
        Genero = jogo.Genero,
        Preco = jogo.Preco,
        DataCadastro = jogo.DataCadastro
    };
    
    public async Task<JogoDto?> AplicarPromocaoAsync(Guid id, decimal novoPreco, CancellationToken ct)
    {
        if (novoPreco <= 0) throw new ArgumentException("Novo preço deve ser maior que zero.");

        var jogo = await _repo.GetByIdAsync(id, ct);
        if (jogo is null) return null;

        jogo.Preco = novoPreco;
        jogo.Touch();

        await _repo.UpdateAsync(jogo, ct);
        return ToDto(jogo);
    }
    
}