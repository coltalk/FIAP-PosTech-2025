using Microsoft.Extensions.Logging;
using UsersApi.Domain.Entities;
using UsersAPI.Application.Interfaces;
using UsersAPI.Domain.Dtos.Request.Role;
using UsersAPI.Domain.Dtos.Responses.Role;
using UsersAPI.Domain.Exceptions;
using UsersAPI.Domain.Interfaces.Services;

namespace UsersAPI.Application.AppServices;

public class RoleAppService : IRoleAppService
{
    private readonly IRoleServices _roleService;
    private readonly ILogger<RoleAppService> _logger;

    public RoleAppService(
        IRoleServices roleService,
        ILogger<RoleAppService> logger)
    {
        _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
        _logger = logger;
    }

    public async Task<RolesResponse> Cadastrar(CadastrarRoleRequest request)
    {
        var entity = new Role
        {
            Id = request.Id,
            RoleName = request.RoleName,
            Description = request.Description
        };
        var criado = await _roleService.Insert(entity);
        if (criado == null)
        {
            _logger.LogError("Falha ao cadastrar role no serviço de domínio | Request: {@Request}", request);
            throw new DomainException("Não foi possível cadastrar a role. Verifique os dados fornecidos.");
        }
        return new RolesResponse
        {
            Id = criado.Id,
            RoleName = criado.RoleName,
            Description = criado.Description
        };
    }

    public async Task<List<RolesResponse>> ListarRoles()
    {
        var roles = _roleService.ListarRoles();
        var rolesResponse = roles.Select(r => new RolesResponse
        {
            Id = r.Id,
            RoleName = r.RoleName,
            Description = r.Description
        }).ToList();
        return await Task.FromResult(rolesResponse);
    }

    public async Task<(RolesResponse? Role, bool Success)> AtualizarRole(AtualizarRoleRequest request)
    {
        var role = new Role
        {
            Id = request.Id,
            RoleName = request.RoleName,
            Description = request.Description
        };
        var (roleAtualizada, sucesso) = await _roleService.AtualizarRole(role);
        if (!sucesso || roleAtualizada == null)
        {
            _logger.LogWarning("Falha ao atualizar role ou role não encontrada | RoleId: {RoleId} | Request: {@Request}", request.Id, request);
            return (null, false);
        }
        var response = new RolesResponse
        {
            Id = roleAtualizada.Id,
            RoleName = roleAtualizada.RoleName,
            Description = roleAtualizada.Description
        };
        return (response, true);
    }
}