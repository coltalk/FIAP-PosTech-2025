using Microsoft.Extensions.Logging;
using UsersApi.Domain.Entities;
using UsersAPI.Domain.Interfaces.Generic;
using UsersAPI.Domain.Interfaces.Repository;
using UsersAPI.Domain.Interfaces.Services;
using UsersAPI.Domain.Services.Generic;

namespace UsersAPI.Domain.Services;

public class UsuarioPerfilServices : GenericServices<UsuarioPerfil>, IUsuarioPerfilService
{
    private readonly IUsuarioPerfilRepository _usuarioPerfilRepository;
    private readonly ILogger<UsuarioPerfilServices> _logger;

    #region Construtor
    public UsuarioPerfilServices(
        IGenericEntityRepository<UsuarioPerfil> repository,
        IUsuarioPerfilRepository usuarioPerfilRepository,
        ILogger<UsuarioPerfilServices> logger) : base(repository)
    {
        _usuarioPerfilRepository = usuarioPerfilRepository;
        _logger = logger;
    }
    #endregion

    public UsuarioPerfil? BuscarPorUsuarioId(Guid usuarioId)
    {
        return _usuarioPerfilRepository.BuscarPorUsuarioId(usuarioId);
    }

    public async Task<UsuarioPerfil> Cadastrar(UsuarioPerfil perfil)
    {
        var perfilCadastrado = await _repository.Insert(perfil);
        if (perfilCadastrado == null)
        {
            _logger.LogError("Falha ao inserir perfil no repositório | UsuarioId: {UsuarioId} | NomeCompleto: {NomeCompleto}", perfil.UsuarioId, perfil.NomeCompleto);
        }
        return perfilCadastrado;
    }

    public async Task<(UsuarioPerfil? Perfil, bool Success)> Atualizar(UsuarioPerfil perfil)
    {
        var perfilExistente = _usuarioPerfilRepository.BuscarPorIdEUsuario(perfil.Id, perfil.UsuarioId);

        if (perfilExistente == null)
        {
            _logger.LogWarning("Perfil não encontrado para atualização | PerfilId: {PerfilId} | UsuarioId: {UsuarioId}", perfil.Id, perfil.UsuarioId);
            return (null, false);
        }

        perfilExistente.NomeCompleto = perfil.NomeCompleto;
        perfilExistente.DataNascimento = perfil.DataNascimento;
        perfilExistente.Pais = perfil.Pais;
        perfilExistente.AvatarUrl = perfil.AvatarUrl;

        var resultado = _repository.Update(perfilExistente);
        if (!resultado.success)
        {
            _logger.LogError("Falha ao atualizar perfil no repositório | PerfilId: {PerfilId} | UsuarioId: {UsuarioId}", perfil.Id, perfil.UsuarioId);
        }
        return await Task.FromResult((resultado.entity, resultado.success));
    }

    public async Task<bool> Deletar(Guid id, Guid usuarioId)
    {
        var perfil = _usuarioPerfilRepository.BuscarPorIdEUsuario(id, usuarioId);

        if (perfil == null)
        {
            _logger.LogWarning("Perfil não encontrado para exclusão | PerfilId: {PerfilId} | UsuarioId: {UsuarioId}", id, usuarioId);
            return false;
        }

        var sucesso = await _repository.DeleteById(id);
        if (!sucesso)
        {
            _logger.LogError("Falha ao deletar perfil no repositório | PerfilId: {PerfilId} | UsuarioId: {UsuarioId}", id, usuarioId);
        }
        return sucesso;
    }
}
