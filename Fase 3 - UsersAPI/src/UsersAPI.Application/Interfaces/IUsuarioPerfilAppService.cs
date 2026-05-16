using UsersAPI.Domain.Dtos.Request.UsuarioPerfil;
using UsersAPI.Domain.Dtos.Responses.UsuarioPerfil;

namespace UsersAPI.Application.Interfaces;

public interface IUsuarioPerfilAppService
{
    Task<BuscarUsuarioPerfilResponse?> BuscarPorUsuarioId(Guid usuarioId);
    Task<BuscarUsuarioPerfilResponse> Cadastrar(CadastrarUsuarioPerfilRequest request);
    Task<(BuscarUsuarioPerfilResponse? Perfil, bool Success)> Atualizar(AtualizarUsuarioPerfilRequest request);
    Task<bool> Deletar(Guid id, Guid usuarioId);
    Task<List<BuscarUsuarioPerfilResponse>> ListarPaginacao(int take, int skip);
}
