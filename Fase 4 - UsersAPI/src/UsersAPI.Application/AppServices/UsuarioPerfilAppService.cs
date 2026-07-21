using Microsoft.Extensions.Logging;
using UsersApi.Domain.Entities;
using UsersAPI.Application.Interfaces;
using UsersAPI.Domain.Dtos.Request.UsuarioPerfil;
using UsersAPI.Domain.Dtos.Responses.UsuarioPerfil;
using UsersAPI.Domain.Exceptions;
using UsersAPI.Domain.Interfaces.Services;

namespace UsersAPI.Application.AppServices;

public class UsuarioPerfilAppService : IUsuarioPerfilAppService
{
    private readonly IUsuarioPerfilService _usuarioPerfilService;
    private readonly IUsuarioService _usuarioService;
    private readonly ILogger<UsuarioPerfilAppService> _logger;

    public UsuarioPerfilAppService(
        IUsuarioPerfilService usuarioPerfilService,
        IUsuarioService usuarioService,
        ILogger<UsuarioPerfilAppService> logger)
    {
        _usuarioPerfilService = usuarioPerfilService;
        _usuarioService = usuarioService;
        _logger = logger;
    }

    public async Task<BuscarUsuarioPerfilResponse?> BuscarPorUsuarioId(Guid usuarioId)
    {
        var usuarioExiste = _usuarioService.GetById(usuarioId);
        if (usuarioExiste == null)
        {
            _logger.LogWarning("Usuário não encontrado ao buscar perfil | UsuarioId: {UsuarioId}", usuarioId);
            throw new NotFoundException($"Usuário com ID {usuarioId} não encontrado.");
        }
        var perfil = _usuarioPerfilService.BuscarPorUsuarioId(usuarioId);
        if (perfil == null)
        {
            _logger.LogWarning("Perfil não encontrado para o usuário | UsuarioId: {UsuarioId}", usuarioId);
            throw new NotFoundException($"Perfil para o usuário com ID {usuarioId} não encontrado.");
        }
        return new BuscarUsuarioPerfilResponse
        {
            Id = perfil.Id,
            UsuarioId = perfil.UsuarioId,
            NomeCompleto = perfil.NomeCompleto,
            DataNascimento = perfil.DataNascimento,
            Pais = perfil.Pais,
            AvatarUrl = perfil.AvatarUrl
        };
    }

    public async Task<List<BuscarUsuarioPerfilResponse>> ListarPaginacao(int take, int skip)
    {
        var usuariosPerfils = await _usuarioPerfilService.ListarPaginacao(take, skip);
        var usuariosResponse = usuariosPerfils.Select(c => new BuscarUsuarioPerfilResponse
        {
            Id = c.Id,
            UsuarioId = c.UsuarioId,
            NomeCompleto = c.NomeCompleto,
            DataNascimento = c.DataNascimento,
            Pais = c.Pais,
            AvatarUrl = c.AvatarUrl
        }).ToList();
        return await Task.FromResult(usuariosResponse);
    }

    public async Task<BuscarUsuarioPerfilResponse> Cadastrar(CadastrarUsuarioPerfilRequest request)
    {
        var usuarioExiste = _usuarioService.GetById(request.UsuarioId);
        if (usuarioExiste == null)
        {
            _logger.LogWarning("Usuário não encontrado para cadastrar perfil | UsuarioId: {UsuarioId}", request.UsuarioId);
            throw new NotFoundException($"Usuário com ID {request.UsuarioId} não encontrado para cadastrar o perfil.");
        }
        var perfil = new UsuarioPerfil(
            request.NomeCompleto,
            request.DataNascimento,
            request.Pais,
            request.AvatarUrl)
        {
            UsuarioId = request.UsuarioId
        };
        var perfilCadastrado = await _usuarioPerfilService.Cadastrar(perfil);
        if (perfilCadastrado == null)
        {
            _logger.LogError("Falha ao cadastrar perfil | UsuarioId: {UsuarioId} | Request: {@Request}", request.UsuarioId, request);
            throw new DomainException("Não foi possível cadastrar o perfil. Verifique os dados fornecidos ou se o usuário já possui um perfil.");
        }
        return new BuscarUsuarioPerfilResponse
        {
            Id = perfilCadastrado.Id,
            UsuarioId = perfilCadastrado.UsuarioId,
            NomeCompleto = perfilCadastrado.NomeCompleto,
            DataNascimento = perfilCadastrado.DataNascimento,
            Pais = perfilCadastrado.Pais,
            AvatarUrl = perfilCadastrado.AvatarUrl
        };
    }

    public async Task<(BuscarUsuarioPerfilResponse? Perfil, bool Success)> Atualizar(AtualizarUsuarioPerfilRequest request)
    {
        var usuarioExiste = _usuarioService.GetById(request.UsuarioId);
        if (usuarioExiste == null)
        {
            _logger.LogWarning("Usuário não encontrado para atualizar perfil | UsuarioId: {UsuarioId} | PerfilId: {PerfilId}", request.UsuarioId, request.Id);
            throw new NotFoundException($"Usuário com ID {request.UsuarioId} não encontrado para atualizar o perfil.");
        }
        var perfil = new UsuarioPerfil(
            request.NomeCompleto,
            request.DataNascimento,
            request.Pais,
            request.AvatarUrl)
        {
            Id = request.Id,
            UsuarioId = request.UsuarioId
        };
        var (perfilAtualizado, sucesso) = await _usuarioPerfilService.Atualizar(perfil);
        if (!sucesso || perfilAtualizado == null)
        {
            _logger.LogWarning("Falha ao atualizar perfil ou perfil não encontrado | PerfilId: {PerfilId} | UsuarioId: {UsuarioId}", request.Id, request.UsuarioId);
            return (null, false);
        }
        var response = new BuscarUsuarioPerfilResponse
        {
            Id = perfilAtualizado.Id,
            UsuarioId = perfilAtualizado.UsuarioId,
            NomeCompleto = perfilAtualizado.NomeCompleto,
            DataNascimento = perfilAtualizado.DataNascimento,
            Pais = perfilAtualizado.Pais,
            AvatarUrl = perfilAtualizado.AvatarUrl
        };
        return (response, true);
    }

    public async Task<bool> Deletar(Guid id, Guid usuarioId)
    {
        var usuarioExiste = _usuarioService.GetById(usuarioId);
        if (usuarioExiste == null)
        {
            _logger.LogWarning("Usuário não encontrado para deletar perfil | UsuarioId: {UsuarioId} | PerfilId: {PerfilId}", usuarioId, id);
            throw new NotFoundException($"Usuário com ID {usuarioId} não encontrado para deletar o perfil.");
        }
        return await _usuarioPerfilService.Deletar(id, usuarioId);
    }
}