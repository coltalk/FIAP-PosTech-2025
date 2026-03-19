using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TcUser.Application.DTOs.Auth;
using TcUser.Application.Interfaces;

namespace TcUser.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    /// <summary>
    /// Registra um novo usuÃ¡rio no sistema.
    /// </summary>
    /// <param name="request">Dados necessÃ¡rios para criaÃ§Ã£o do usuÃ¡rio.</param>
    /// <param name="ct">Token para cancelamento da requisiÃ§Ã£o.</param>
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken ct)
    {
        await _auth.RegisterAsync(request, ct);
        return NoContent();
    }

    /// <summary>
    /// Realiza o login do usuÃ¡rio e retorna o token JWT.
    /// </summary>
    /// <param name="request">Credenciais do usuÃ¡rio.</param>
    /// <param name="ct">Token para cancelamento da requisiÃ§Ã£o.</param>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        var token = await _auth.LoginAsync(request, ct);
        return Ok(token);
    }

    /// <summary>
    /// Retorna as informaÃ§Ãµes do usuÃ¡rio autenticado a partir do token JWT.
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            Id = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                 ?? User.FindFirstValue("sub"),

            Email = User.FindFirstValue(ClaimTypes.Email) 
                    ?? User.FindFirstValue("email"),

            Name = User.FindFirstValue("name"),

            Role = User.FindFirstValue(ClaimTypes.Role)
        });
    }

    /// <summary>
    /// Endpoint acessÃ­vel apenas por usuÃ¡rios com a role Admin.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("admin-only")]
    public IActionResult AdminOnly()
        => Ok(new
        {
            Message = "VocÃª Ã© Admin e acessou um endpoint protegido por role."
        });
}
