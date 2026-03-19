using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TcCatalog.Application.DTOs;
using TcCatalog.Application.Interfaces;

namespace TcCatalog.Api.Controllers;

/// <summary>
/// Endpoints administrativos para gerenciar a biblioteca (jogos atribuídos) de usuários.
/// </summary>
[ApiController]
[Route("api/admin/biblioteca")]
[Authorize(Roles = "Admin")]
public class AdminBibliotecaController : ControllerBase
{
    private readonly IBibliotecaJogoService _service;

    public AdminBibliotecaController(IBibliotecaJogoService service) => _service = service;

    /// <summary>
    /// Atribui um jogo à biblioteca de um usuário (por identificador).
    /// </summary>
    /// <param name="dto">Identificador do usuário e ID do jogo a ser vinculado.</param>
    [HttpPost]
    public async Task<IActionResult> Atribuir([FromBody] VincularJogoUsuarioDto dto, CancellationToken ct)
    {
        await _service.AddJogoAoUsuarioAsync(dto.UserId, dto.JogoId, ct);
        return NoContent();
    }

    /// <summary>
    /// Lista os jogos da biblioteca de um usuário (por identificador).
    /// </summary>
    /// <param name="userId">Identificador do usuário.</param>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BibliotecaJogoDto>>> Listar([FromQuery] Guid userId, CancellationToken ct)
        => Ok(await _service.GetJogosDoUsuarioAsync(userId, ct));

    /// <summary>
    /// Remove um jogo da biblioteca de um usuário (por identificador).
    /// </summary>
    /// <param name="userId">Identificador do usuário.</param>
    /// <param name="jogoId">ID do jogo.</param>
    [HttpDelete]
    public async Task<IActionResult> Remover([FromQuery] Guid userId, [FromQuery] Guid jogoId, CancellationToken ct)
        => (await _service.RemoveJogoDoUsuarioAsync(userId, jogoId, ct)) ? NoContent() : NotFound();

    /// <summary>
    /// Remove todos os jogos da biblioteca de um usuário (por identificador).
    /// </summary>
    /// <param name="userId">Identificador do usuário.</param>
    [HttpDelete("todos")]
    public async Task<IActionResult> RemoverTodos([FromQuery] Guid userId, CancellationToken ct)
        => (await _service.RemoveTodosJogosDoUsuarioAsync(userId, ct)) > 0 ? NoContent() : NotFound();
}
