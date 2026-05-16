using Microsoft.Extensions.Logging;
using UsersApi.Domain.Entities;
using UsersAPI.Domain.Interfaces.Generic;
using UsersAPI.Domain.Interfaces.Repository;
using UsersAPI.Domain.Interfaces.Services;
using UsersAPI.Domain.Services.Generic;

namespace UsersAPI.Domain.Services;

public class ContatoService : GenericServices<Contato>, IContatoService
{
    private readonly IContatoRepository _contatoRepository;
    private readonly ILogger<ContatoService> _logger;

    public ContatoService(
        IGenericEntityRepository<Contato> repository,
        IContatoRepository contatoRepository,
        ILogger<ContatoService> logger) : base(repository)
    {
        _contatoRepository = contatoRepository;
        _logger = logger;
    }

    public List<Contato> ListarPorUsuario(Guid usuarioId)
    {
        return _contatoRepository.ListarPorUsuario(usuarioId);
    }

    public async Task<Contato> Cadastrar(Contato contato)
    {
        var contatoCadastrado = await _repository.Insert(contato);
        if (contatoCadastrado == null)
        {
            _logger.LogError("Falha ao inserir contato no repositório | UsuarioId: {UsuarioId} | Celular: {Celular} | Email: {Email}", contato.UsuarioId, contato.Celular, contato.Email);
        }
        return contatoCadastrado;
    }

    public async Task<(Contato? Contato, bool Success)> Atualizar(Contato contato)
    {
        var contatoExistente = _contatoRepository.BuscarPorIdEUsuario(contato.Id, contato.UsuarioId);
        if (contatoExistente == null)
        {
            _logger.LogWarning("Contato não encontrado para atualização | ContatoId: {ContatoId} | UsuarioId: {UsuarioId}", contato.Id, contato.UsuarioId);
            return (null, false);
        }
        contatoExistente.Celular = contato.Celular;
        contatoExistente.Email = contato.Email;
        var resultado = _repository.Update(contatoExistente);
        if (!resultado.success)
        {
            _logger.LogError("Falha ao atualizar contato no repositório | ContatoId: {ContatoId} | UsuarioId: {UsuarioId}", contato.Id, contato.UsuarioId);
        }
        return await Task.FromResult((resultado.entity, resultado.success));
    }

    public async Task<bool> Deletar(Guid id, Guid usuarioId)
    {
        var contato = _contatoRepository.BuscarPorIdEUsuario(id, usuarioId);
        if (contato == null)
        {
            _logger.LogWarning("Contato não encontrado para exclusão | ContatoId: {ContatoId} | UsuarioId: {UsuarioId}", id, usuarioId);
            return false;
        }
        var sucesso = await _repository.DeleteById(id);
        if (!sucesso)
        {
            _logger.LogError("Falha ao deletar contato no repositório | ContatoId: {ContatoId} | UsuarioId: {UsuarioId}", id, usuarioId);
        }
        return sucesso;
    }

}
