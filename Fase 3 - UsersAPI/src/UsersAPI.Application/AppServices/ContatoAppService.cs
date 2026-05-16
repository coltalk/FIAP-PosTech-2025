using Microsoft.Extensions.Logging;
using UsersApi.Domain.Entities;
using UsersAPI.Application.Interfaces;
using UsersAPI.Domain.Dtos.Request.Contato;
using UsersAPI.Domain.Dtos.Responses.Contato;
using UsersAPI.Domain.Exceptions;
using UsersAPI.Domain.Interfaces.Services;

namespace UsersAPI.Application.AppServices;

public class ContatoAppService : IContatoAppService
{
    private readonly IContatoService _contatoService;
    private readonly IUsuarioService _usuarioService;
    private readonly ILogger<ContatoAppService> _logger;

    public ContatoAppService(
        IContatoService contatoService,
        IUsuarioService usuarioService,
        ILogger<ContatoAppService> logger)
    {
        _contatoService = contatoService;
        _usuarioService = usuarioService;
        _logger = logger;
    }

    public async Task<List<ContatoResponse>> ListarPorUsuario(Guid usuarioId)
    {
        var usuarioExiste = _usuarioService.GetById(usuarioId);
        if (usuarioExiste == null)
        {
            _logger.LogWarning("Usuário não encontrado ao listar contatos | UsuarioId: {UsuarioId}", usuarioId);
            throw new NotFoundException($"Usuário com ID {usuarioId} não encontrado.");
        }
        var contatos = _contatoService.ListarPorUsuario(usuarioId);
        var contatosResponse = contatos.Select(c => new ContatoResponse
        {
            Id = c.Id,
            UsuarioId = c.UsuarioId,
            Celular = c.Celular,
            Email = c.Email
        }).ToList();
        return await Task.FromResult(contatosResponse);
    }

    public async Task<List<ContatoResponse>> ListarPaginacao(int take, int skip)
    {
        var contatos = await _contatoService.ListarPaginacao(take, skip);
        var contatosResponse = contatos.Select(c => new ContatoResponse
        {
            Id = c.Id,
            UsuarioId = c.UsuarioId,
            Celular = c.Celular,
            Email = c.Email
        }).ToList();
        return await Task.FromResult(contatosResponse);
    }

    public async Task<ContatoResponse> Cadastrar(CadastrarContatoRequest request)
    {
        var usuarioExiste = _usuarioService.GetById(request.UsuarioId);
        if (usuarioExiste == null)
        {
            _logger.LogWarning("Usuário não encontrado para cadastrar contato | UsuarioId: {UsuarioId}", request.UsuarioId);
            throw new NotFoundException($"Usuário com ID {request.UsuarioId} não encontrado para cadastrar o contato.");
        }
        var contato = new Contato(request.Celular, request.Email)
        {
            UsuarioId = request.UsuarioId
        };
        var contatoCadastrado = await _contatoService.Cadastrar(contato);
        if (contatoCadastrado == null)
        {
            _logger.LogError("Falha ao cadastrar contato | UsuarioId: {UsuarioId} | Request: {@Request}", request.UsuarioId, request);
            throw new DomainException("Não foi possível cadastrar o contato. Verifique os dados fornecidos.");
        }
        return new ContatoResponse
        {
            Id = contatoCadastrado.Id,
            UsuarioId = contatoCadastrado.UsuarioId,
            Celular = contatoCadastrado.Celular,
            Email = contatoCadastrado.Email
        };
    }

    public async Task<(ContatoResponse? Contato, bool Success)> Atualizar(AtualizarContatoRequest request)
    {
        var usuarioExiste = _usuarioService.GetById(request.UsuarioId);
        if (usuarioExiste == null)
        {
            _logger.LogWarning("Usuário não encontrado para atualizar contato | UsuarioId: {UsuarioId} | ContatoId: {ContatoId}", request.UsuarioId, request.Id);
            throw new NotFoundException($"Usuário com ID {request.UsuarioId} não encontrado para atualizar o contato.");
        }
        var contato = new Contato(request.Celular, request.Email)
        {
            Id = request.Id,
            UsuarioId = request.UsuarioId
        };
        var (contatoAtualizado, sucesso) = await _contatoService.Atualizar(contato);
        if (!sucesso || contatoAtualizado == null)
        {
            _logger.LogWarning("Falha ao atualizar contato ou contato não encontrado | ContatoId: {ContatoId} | UsuarioId: {UsuarioId}", request.Id, request.UsuarioId);
            return (null, false);
        }
        var response = new ContatoResponse
        {
            Id = contatoAtualizado.Id,
            UsuarioId = contatoAtualizado.UsuarioId,
            Celular = contatoAtualizado.Celular,
            Email = contatoAtualizado.Email
        };
        return (response, true);
    }

    public async Task<bool> Deletar(Guid id, Guid usuarioId)
    {
        var usuarioExiste = _usuarioService.GetById(usuarioId);
        if (usuarioExiste == null)
        {
            _logger.LogWarning("Usuário não encontrado para deletar contato | UsuarioId: {UsuarioId} | ContatoId: {ContatoId}", usuarioId, id);
            throw new NotFoundException($"Usuário com ID {usuarioId} não encontrado para deletar o contato.");
        }
        return await _contatoService.Deletar(id, usuarioId);
    }
}
