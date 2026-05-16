using Microsoft.Extensions.Logging;
using UsersApi.Domain.Entities;
using UsersAPI.Domain.Interfaces.Generic;
using UsersAPI.Domain.Interfaces.Repository;
using UsersAPI.Domain.Interfaces.Services;
using UsersAPI.Domain.Services.Generic;

namespace UsersAPI.Domain.Services;

public class EnderecoService : GenericServices<Endereco>, IEnderecoService
{
    private readonly IEnderecoRepository _enderecoRepository;
    private readonly ILogger<EnderecoService> _logger;

    public EnderecoService(
        IGenericEntityRepository<Endereco> repository,
        IEnderecoRepository enderecoRepository,
        ILogger<EnderecoService> logger) : base(repository)
    {
        _enderecoRepository = enderecoRepository;
        _logger = logger;
    }

    public List<Endereco> ListarPorUsuario(Guid usuarioId)
    {
        return _enderecoRepository.ListarPorUsuario(usuarioId);
    }

    public async Task<Endereco> Cadastrar(Endereco endereco)
    {
        endereco.Id = Guid.NewGuid();
        var enderecoCadastrado = await _repository.Insert(endereco);
        if (enderecoCadastrado == null)
        {
            _logger.LogError("Falha ao inserir endereço no repositório | UsuarioId: {UsuarioId} | Rua: {Rua} | Cep: {Cep}", endereco.UsuarioId, endereco.Rua, endereco.Cep);
        }
        return enderecoCadastrado;
    }

    public async Task<(Endereco? Endereco, bool Success)> Atualizar(Endereco endereco)
    {
        var enderecoExistente = _enderecoRepository.BuscarPorIdEUsuario(endereco.Id, endereco.UsuarioId);
        if (enderecoExistente == null)
        {
            _logger.LogWarning("Endereço não encontrado para atualização | EnderecoId: {EnderecoId} | UsuarioId: {UsuarioId}", endereco.Id, endereco.UsuarioId);
            return (null, false);
        }
        enderecoExistente.Rua = endereco.Rua;
        enderecoExistente.Numero = endereco.Numero;
        enderecoExistente.Complemento = endereco.Complemento;
        enderecoExistente.Bairro = endereco.Bairro;
        enderecoExistente.Cidade = endereco.Cidade;
        enderecoExistente.Estado = endereco.Estado;
        enderecoExistente.Cep = endereco.Cep;
        var resultado = _repository.Update(enderecoExistente);
        if (!resultado.success)
        {
            _logger.LogError("Falha ao atualizar endereço no repositório | EnderecoId: {EnderecoId} | UsuarioId: {UsuarioId}", endereco.Id, endereco.UsuarioId);
        }
        return await Task.FromResult((resultado.entity, resultado.success));
    }

    public async Task<bool> Deletar(Guid id, Guid usuarioId)
    {
        var endereco = _enderecoRepository.BuscarPorIdEUsuario(id, usuarioId);
        if (endereco == null)
        {
            _logger.LogWarning("Endereço não encontrado para exclusão | EnderecoId: {EnderecoId} | UsuarioId: {UsuarioId}", id, usuarioId);
            return false;
        }
        var sucesso = await _repository.DeleteById(id);
        if (!sucesso)
        {
            _logger.LogError("Falha ao deletar endereço no repositório | EnderecoId: {EnderecoId} | UsuarioId: {UsuarioId}", id, usuarioId);
        }
        return sucesso;
    }
}
