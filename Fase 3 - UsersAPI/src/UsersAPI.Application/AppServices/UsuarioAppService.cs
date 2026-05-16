using Microsoft.Extensions.Logging;
using UsersAPI.Application.Interfaces;
using UsersAPI.Domain.Dtos.Request.Usuario;
using UsersAPI.Domain.Dtos.Responses.Usuario;
using UsersAPI.Domain.Events;
using UsersAPI.Domain.Exceptions;
using UsersAPI.Domain.Interfaces.Events;
using UsersAPI.Domain.Interfaces.Services;

namespace UsersAPI.Application.AppServices;

public class UsuarioAppService : IUsuarioAppService
{
    private readonly IUsuarioService _usuarioService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<UsuarioAppService> _logger;

    public UsuarioAppService(
    IUsuarioService usuarioService,
    IEventPublisher eventPublisher,
    ILogger<UsuarioAppService> logger)
    {
        _usuarioService = usuarioService;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<CadastrarUsuarioResponse> Cadastrar(CadastrarUsuarioRequest request)
    {
        var cadastroUsuarioResult = await _usuarioService.CadastrarUsuario(request);

        if (cadastroUsuarioResult == null)
        {
            _logger.LogError("Falha ao cadastrar usuário no serviço de domínio | Email: {Email}", request.Email);
            throw new DomainException("Não foi possível cadastrar o usuário. Verifique os dados fornecidos.");
        }

        try
        {
            var userCreatedEvent = new UserCreatedEvent
            {
                UsuarioId = cadastroUsuarioResult.Id
            };

            await _eventPublisher.PublishUserCreatedAsync(userCreatedEvent);

            _logger.LogInformation(
                "Evento UserCreated publicado | UsuarioId: {UsuarioId}",
                cadastroUsuarioResult.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar UserCreated, mas usuário foi cadastrado | UsuarioId: {UsuarioId}", cadastroUsuarioResult.Id);
        }

        return new CadastrarUsuarioResponse
        {
            IdUsuario = cadastroUsuarioResult.Id
        };
    }

    public BuscarPorIdResponse BuscarPorId(Guid id)
    {
        var usuario = _usuarioService.GetById(id);
        if (usuario is null)
        {
            _logger.LogWarning("Usuário não encontrado | UsuarioId: {UsuarioId}", id);
            throw new NotFoundException("Usuário não encontrado.");
        }
        var result = new BuscarPorIdResponse
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Ativo = usuario.Ativo,
            DataCriacao = usuario.DataCriacao,
            DataAtualizacao = usuario.DataAtualizacao,
            Perfil = usuario.Perfil != null ? new UsuarioPerfilResponse
            {
                Id = usuario.Perfil.Id,
                NomeCompleto = usuario.Perfil.NomeCompleto,
                DataNascimento = usuario.Perfil.DataNascimento,
                Pais = usuario.Perfil.Pais,
                AvatarUrl = usuario.Perfil.AvatarUrl,
            } : null,
            Roles = usuario.UsuarioRoles?.Select(ur => new UsuarioRoleResponse
            {
                Id = ur.Id,
                RoleId = ur.RoleId,
                RoleName = ur.Role?.RoleName,
                Description = ur.Role?.Description,
            }).ToList() ?? []
        };
        return result;
    }

    public async Task<bool> AlterarSenha(AlterarSenhaRequest request)
    {
        var sucesso = await _usuarioService.AlterarSenha(request);
        if (!sucesso)
        {
            _logger.LogWarning("Falha ao alterar senha: Usuário não encontrado ou senha atual incorreta | UsuarioId: {UsuarioId}", request.Id);
        }
        return sucesso;
    }

    public async Task<AlterarStatusResponse> AlterarStatus(Guid id)
    {
        var result = await _usuarioService.AlterarStatus(id);
        if (result == null)
        {
            _logger.LogWarning("Falha ao alterar status: Usuário não encontrado | UsuarioId: {UsuarioId}", id);
        }
        return result;
    }
}
