using Microsoft.Extensions.Logging;
using UsersApi.Domain.Entities;
using UsersAPI.Application.Interfaces;
using UsersAPI.Domain.Dtos.Request.Enderecos;
using UsersAPI.Domain.Dtos.Responses.Endereco;
using UsersAPI.Domain.Exceptions;
using UsersAPI.Domain.Interfaces.Services;

namespace UsersAPI.Application.AppServices;

public class EnderecoAppService : IEnderecoAppService
{
    private readonly IEnderecoService _enderecoService;
    private readonly IUsuarioService _usuarioService;
    private readonly ILogger<EnderecoAppService> _logger;

    public EnderecoAppService(
        IEnderecoService enderecoService,
        IUsuarioService usuarioService,
        ILogger<EnderecoAppService> logger)
    {
        _enderecoService = enderecoService;
        _usuarioService = usuarioService;
        _logger = logger;
    }

    public async Task<List<EnderecoResponse>> ListarPorUsuario(Guid usuarioId)
    {
        var usuarioExiste = _usuarioService.GetById(usuarioId);
        if (usuarioExiste == null)
        {
            _logger.LogWarning("Usuário não encontrado ao listar endereços | UsuarioId: {UsuarioId}", usuarioId);
            throw new NotFoundException($"Usuário com ID {usuarioId} não encontrado.");
        }
        var enderecos = _enderecoService.ListarPorUsuario(usuarioId);
        var enderecosResponse = enderecos.Select(e => new EnderecoResponse
        {
            Id = e.Id,
            UsuarioId = e.UsuarioId,
            Rua = e.Rua,
            Numero = e.Numero,
            Complemento = e.Complemento,
            Bairro = e.Bairro,
            Cidade = e.Cidade,
            Estado = e.Estado,
            Cep = e.Cep
        }).ToList();
        return await Task.FromResult(enderecosResponse);
    }

    public async Task<List<EnderecoResponse>> ListarPaginacao(int take, int skip)
    {
        var enderecos = await _enderecoService.ListarPaginacao(take, skip);
        var enderecosResponse = enderecos.Select(e => new EnderecoResponse
        {
            Id = e.Id,
            UsuarioId = e.UsuarioId,
            Rua = e.Rua,
            Numero = e.Numero,
            Complemento = e.Complemento,
            Bairro = e.Bairro,
            Cidade = e.Cidade,
            Estado = e.Estado,
            Cep = e.Cep
        }).ToList();
        return await Task.FromResult(enderecosResponse);
    }

    public async Task<EnderecoResponse> Cadastrar(CadastrarEnderecoRequest request)
    {
        var usuarioExiste = _usuarioService.GetById(request.UsuarioId);
        if (usuarioExiste == null)
        {
            _logger.LogWarning("Usuário não encontrado para cadastrar endereço | UsuarioId: {UsuarioId}", request.UsuarioId);
            throw new NotFoundException($"Usuário com ID {request.UsuarioId} não encontrado para cadastrar o endereço.");
        }
        var endereco = new Endereco
        {
            UsuarioId = request.UsuarioId,
            Rua = request.Rua,
            Numero = request.Numero,
            Complemento = request.Complemento,
            Bairro = request.Bairro,
            Cidade = request.Cidade,
            Estado = request.Estado,
            Cep = request.Cep
        };
        var enderecoCadastrado = await _enderecoService.Cadastrar(endereco);
        if (enderecoCadastrado == null)
        {
            _logger.LogError("Falha ao cadastrar endereço | UsuarioId: {UsuarioId} | Request: {@Request}", request.UsuarioId, request);
            throw new DomainException("Não foi possível cadastrar o endereço. Verifique os dados fornecidos.");
        }
        return new EnderecoResponse
        {
            Id = enderecoCadastrado.Id,
            UsuarioId = enderecoCadastrado.UsuarioId,
            Rua = enderecoCadastrado.Rua,
            Numero = enderecoCadastrado.Numero,
            Complemento = enderecoCadastrado.Complemento,
            Bairro = enderecoCadastrado.Bairro,
            Cidade = enderecoCadastrado.Cidade,
            Estado = enderecoCadastrado.Estado,
            Cep = enderecoCadastrado.Cep
        };
    }

    public async Task<(EnderecoResponse? Endereco, bool Success)> Atualizar(AtualizarEnderecoRequest request)
    {
        var usuarioExiste = _usuarioService.GetById(request.UsuarioId);
        if (usuarioExiste == null)
        {
            _logger.LogWarning("Usuário não encontrado para atualizar endereço | UsuarioId: {UsuarioId} | EnderecoId: {EnderecoId}", request.UsuarioId, request.Id);
            throw new NotFoundException($"Usuário com ID {request.UsuarioId} não encontrado para atualizar o endereço.");
        }
        var endereco = new Endereco
        {
            Id = request.Id,
            UsuarioId = request.UsuarioId,
            Rua = request.Rua,
            Numero = request.Numero,
            Complemento = request.Complemento,
            Bairro = request.Bairro,
            Cidade = request.Cidade,
            Estado = request.Estado,
            Cep = request.Cep
        };
        var (enderecoAtualizado, sucesso) = await _enderecoService.Atualizar(endereco);
        if (!sucesso || enderecoAtualizado == null)
        {
            _logger.LogWarning("Falha ao atualizar endereço ou endereço não encontrado | EnderecoId: {EnderecoId} | UsuarioId: {UsuarioId}", request.Id, request.UsuarioId);
            return (null, false);
        }
        var response = new EnderecoResponse
        {
            Id = enderecoAtualizado.Id,
            UsuarioId = enderecoAtualizado.UsuarioId,
            Rua = enderecoAtualizado.Rua,
            Numero = enderecoAtualizado.Numero,
            Complemento = enderecoAtualizado.Complemento,
            Bairro = enderecoAtualizado.Bairro,
            Cidade = enderecoAtualizado.Cidade,
            Estado = enderecoAtualizado.Estado,
            Cep = enderecoAtualizado.Cep
        };
        return (response, true);
    }

    public async Task<bool> Deletar(Guid id, Guid usuarioId)
    {
        var usuarioExiste = _usuarioService.GetById(usuarioId);
        if (usuarioExiste == null)
        {
            _logger.LogWarning("Usuário não encontrado para deletar endereço | UsuarioId: {UsuarioId} | EnderecoId: {EnderecoId}", usuarioId, id);
            throw new NotFoundException($"Usuário com ID {usuarioId} não encontrado para deletar o endereço.");
        }
        return await _enderecoService.Deletar(id, usuarioId);
    }
}
