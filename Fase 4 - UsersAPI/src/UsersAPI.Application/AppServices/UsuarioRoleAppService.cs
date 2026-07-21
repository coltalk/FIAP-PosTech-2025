using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UsersAPI.Application.Interfaces;
using UsersAPI.Domain.Dtos.Request.UsuarioRole;
using UsersAPI.Domain.Dtos.Responses.UsuarioRole;
using UsersAPI.Domain.Exceptions;
using UsersAPI.Domain.Interfaces.Services;

namespace UsersAPI.Application.AppServices;

public class UsuarioRoleAppService : IUsuarioRoleAppService
{
    private readonly IUsuarioRoleServices _usuarioRoleService;
    private readonly IUsuarioService _usuarioService;
    private readonly IRoleServices _roleService;
    private readonly ILogger<UsuarioRoleAppService> _logger;

    public UsuarioRoleAppService(
        IUsuarioRoleServices usuarioRoleService,
        IUsuarioService usuarioService,
        IRoleServices roleService,
        ILogger<UsuarioRoleAppService> logger)
    {
        _usuarioRoleService = usuarioRoleService ?? throw new ArgumentNullException(nameof(usuarioRoleService));
        _usuarioService = usuarioService ?? throw new ArgumentNullException(nameof(usuarioService));
        _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
        _logger = logger;
    }

    public async Task<bool> AlterarRoleUsuario(AlterarUsuarioRoleRequest request)
    {
        var usuarioExiste = _usuarioService.GetById(request.UsuarioId);
        if (usuarioExiste == null)
        {
            _logger.LogWarning("Usuário não encontrado ao tentar alterar role | UsuarioId: {UsuarioId} | IdUsuarioRole: {IdUsuarioRole}", request.UsuarioId, request.IdUsuarioRole);
            throw new NotFoundException($"Usuário com ID {request.UsuarioId} não encontrado.");
        }
        var roleExiste = _roleService.GetByIdInt((int)request.TipoUsuario);
        if (roleExiste == null)
        {
            _logger.LogWarning("Role não encontrada ao tentar alterar role | RoleId: {RoleId} | UsuarioId: {UsuarioId}", (int)request.TipoUsuario, request.UsuarioId);
            throw new NotFoundException($"Role com ID {(int)request.TipoUsuario} não encontrada.");
        }
        var usuarioRoleExistente = _usuarioRoleService.GetById(request.IdUsuarioRole);
        if (usuarioRoleExistente == null)
        {
            _logger.LogWarning("Associação Usuário-Role não encontrada para atualização | IdUsuarioRole: {IdUsuarioRole} | UsuarioId: {UsuarioId}", request.IdUsuarioRole, request.UsuarioId);
            throw new NotFoundException($"Associação Usuário-Role com ID {request.IdUsuarioRole} não encontrada.");
        }
        usuarioRoleExistente.UsuarioId = request.UsuarioId;
        usuarioRoleExistente.RoleId = (int)request.TipoUsuario;
        var (updatedEntity, sucesso) = await _usuarioRoleService.Update(usuarioRoleExistente);
        if (!sucesso)
        {
            _logger.LogError("Falha ao alterar a role do usuário no serviço de domínio | IdUsuarioRole: {IdUsuarioRole} | UsuarioId: {UsuarioId} | NovaRoleId: {NovaRoleId}", request.IdUsuarioRole, request.UsuarioId, (int)request.TipoUsuario);
            throw new DomainException("Não foi possível alterar a role do usuário. Verifique os dados fornecidos.");
        }
        return true;
    }

    public async Task<IEnumerable<ListarRolesPorUsuarioResponse>> ListarRolesPorUsuario(ListarRolePorUsuarioRequest request)
    {
        var usuarioExiste = _usuarioService.GetById(request.UsuarioId);
        if (usuarioExiste == null)
        {
            _logger.LogWarning("Usuário não encontrado ao listar roles por usuário | UsuarioId: {UsuarioId}", request.UsuarioId);
            throw new NotFoundException($"Usuário com ID {request.UsuarioId} não encontrado.");
        }
        var listaRolesUsuario = await _usuarioRoleService.Get()
            .Include(ur => ur.Role)
            .Where(x => x.UsuarioId == request.UsuarioId)
            .AsNoTracking()
            .ToListAsync();
        var response = listaRolesUsuario.Select(x => new ListarRolesPorUsuarioResponse
        {
            Id = x.Id,
            UsuarioId = x.UsuarioId,
            RoleId = x.Role.Id,
            RoleName = x.Role.RoleName,
            Description = x.Role.Description
        }).ToList();
        return response;
    }
}