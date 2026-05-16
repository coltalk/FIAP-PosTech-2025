using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UsersApi.Domain.Entities;
using UsersAPI.Domain.Dtos.Request.Usuario;
using UsersAPI.Domain.Dtos.Responses.Usuario;
using UsersAPI.Domain.Exceptions;
using UsersAPI.Domain.Interfaces.Generic;
using UsersAPI.Domain.Interfaces.Services;
using UsersAPI.Domain.Services.Generic;

namespace UsersAPI.Domain.Services;

public class UsuarioServices : GenericServices<Usuario>, IUsuarioService
{
    private readonly ILogger<UsuarioServices> _logger;

    public UsuarioServices(
        IGenericEntityRepository<Usuario> repository,
        ILogger<UsuarioServices> logger) : base(repository)
    {
        _logger = logger;
    }

    public async Task<Usuario> CadastrarUsuario(CadastrarUsuarioRequest request)
    {
        var usuarioExistente = Get().Where(u => u.Nome == request.Nome).FirstOrDefault();
        if (usuarioExistente != null)
        {
            _logger.LogWarning("Tentativa de cadastro com nome já existente: {Nome}", request.Nome);
            throw new DomainException($"Usuário com o nome '{request.Nome}' já existe.");
        }

        var senhaCriptografada = BCrypt.Net.BCrypt.HashPassword(request.Senha);
        var usuario = Usuario.Criar(request.Nome, senhaCriptografada);
        usuario.Contatos = [new Contato(request.Celular, request.Email)];
        usuario.UsuarioRoles = [new UsuarioRole((int)request.TipoUsuario)];
        usuario.Perfil = new UsuarioPerfil(request.NomeCompleto, request.DataNascimento, request.Pais, request.AvatarUrl);

        try
        {
            var usuarioCriado = await Insert(usuario);
            return usuarioCriado;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Erro de banco de dados ao cadastrar usuário");
            throw new DomainException("Não foi possível cadastrar o usuário devido a um problema de dados. Detalhes: " + ex.InnerException?.Message, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao cadastrar usuário");
            throw new DomainException("Ocorreu um erro inesperado ao cadastrar o usuário. Detalhes: " + ex.Message, ex);
        }
    }

    public async Task<Usuario> ValidarLogin(string usuario, string senha)
    {
        var usuarioResult = Get().Where(u => u.Nome == usuario).FirstOrDefault();

        if (usuarioResult == null)
        {
            _logger.LogWarning("Usuário não encontrado na validação de login");
            throw new AutenticacaoException("Usuário não encontrado.");
        }

        if (!BCrypt.Net.BCrypt.Verify(senha, usuarioResult.Senha))
        {
            _logger.LogWarning("Senha incorreta fornecida | Id: {Id}", usuarioResult.Id);
            throw new AutenticacaoException("Senha incorreta.");
        }

        if (!usuarioResult.Ativo)
        {
            _logger.LogWarning("Tentativa de login com usuário inativo | Id: {Id}", usuarioResult.Id);
            throw new AutenticacaoException("Usuário inativo.");
        }

        return usuarioResult;
    }

    public async Task<bool> AlterarSenha(AlterarSenhaRequest request)
    {
        var usuarioResult = Get().Where(u => u.Id == request.Id).FirstOrDefault();

        if (usuarioResult == null)
        {
            _logger.LogWarning("Usuário não encontrado para alteração de senha | Id: {Id}", request.Id);
            throw new NotFoundException("Usuário não encontrado.");
        }

        var senhaCriptografada = BCrypt.Net.BCrypt.HashPassword(request.Senha);
        usuarioResult.AlterarSenha(senhaCriptografada);

        var (usuarioAtualizado, success) = await Update(usuarioResult);

        if (!success)
        {
            _logger.LogWarning("Falha ao alterar senha | Id: {Id}", request.Id);
        }

        return success;
    }

    public async Task<AlterarStatusResponse> AlterarStatus(Guid Id)
    {
        var usuarioResult = Get().Where(u => u.Id == Id).FirstOrDefault();

        if (usuarioResult == null)
        {
            _logger.LogWarning("Usuário não encontrado para alteração de status | Id: {Id}", Id);
            throw new NotFoundException("Usuário não encontrado.");
        }

        var novoStatus = !usuarioResult.Ativo;
        usuarioResult.AlterarStatus(novoStatus);

        var (usuarioAtualizado, success) = await Update(usuarioResult);

        if (!success || usuarioAtualizado == null)
        {
            _logger.LogError("Falha ao alterar status do usuário | Id: {Id}", Id);
            throw new DomainException("Não foi possível alterar o status do usuário.");
        }

        return new AlterarStatusResponse(usuarioAtualizado.Ativo ? "Ativo" : "Inativo");
    }
}
