using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TcCatalog.API.Services.Interfaces;
using TcCatalog.Application.DTOs;
using TcCatalog.Application.Interfaces;

namespace TcCatalog.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/user/biblioteca")]
public class BibliotecaController : ControllerBase
{
    private readonly IBibliotecaJogoService _service;
    private readonly ICurrentUser _currentUser;

    public BibliotecaController(IBibliotecaJogoService service, ICurrentUser currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Retorna os jogos aprovados da biblioteca do usuario autenticado.
    /// </summary>
    /// <param name="ct">Token para cancelamento da requisicao.</param>
    [HttpGet("me")]
    public async Task<ActionResult<IReadOnlyList<MinhaBibliotecaJogoDto>>> MinhaBiblioteca(
        CancellationToken ct)
    {
        var jogos = await _service.GetJogosAprovadosDoUsuarioAsync(_currentUser.UserId.Value, ct);
        return Ok(jogos);
    }
}
