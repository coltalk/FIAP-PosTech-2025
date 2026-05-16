using Microsoft.Extensions.Logging;
using UsersAPI.Application.Interfaces;
using UsersAPI.Domain.Dtos.Responses.Authentication;
using UsersAPI.Domain.Exceptions;
using UsersAPI.Domain.Interfaces.Services;

namespace UsersAPI.Application.AppServices;

public class AuthenticationAppService : IAuthenticationAppService
{
    private readonly IJwtGenerator _jwtGenerator;
    private readonly IUsuarioService _usuarioService;
    private readonly ILogger<AuthenticationAppService> _logger;

    public AuthenticationAppService(
        IJwtGenerator jwtGenerator,
        IUsuarioService usuarioService,
        ILogger<AuthenticationAppService> logger)
    {
        _jwtGenerator = jwtGenerator;
        _usuarioService = usuarioService;
        _logger = logger;
    }

    public async Task<LoginResponse> Login(string usuario, string senha)
    {
        try
        {
            var usuarioResult = await _usuarioService.ValidarLogin(usuario, senha);

            if (usuarioResult == null)
            {
                _logger.LogWarning("Credenciais inválidas fornecidas");
                throw new AutenticacaoException("Usuário ou senha inválidos.");
            }

            if (!usuarioResult.Ativo)
            {
                _logger.LogWarning("Tentativa de login com usuário inativo | UserId: {UserId}", usuarioResult.Id);
                throw new AutenticacaoException("Usuário inativo.");
            }

            var tokenJwt = _jwtGenerator.GenerateToken(usuarioResult);

            return new LoginResponse(tokenJwt);
        }
        catch (AutenticacaoException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao processar autenticação");
            throw new AutenticacaoException("Erro ao processar autenticação.");
        }
    }
}
