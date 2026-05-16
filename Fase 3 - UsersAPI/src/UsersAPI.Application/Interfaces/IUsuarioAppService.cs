using UsersAPI.Domain.Dtos.Request.Usuario;
using UsersAPI.Domain.Dtos.Responses.Usuario;

namespace UsersAPI.Application.Interfaces;

public interface IUsuarioAppService
{
    BuscarPorIdResponse BuscarPorId(Guid id);
    Task<CadastrarUsuarioResponse> Cadastrar(CadastrarUsuarioRequest request);
    Task<bool> AlterarSenha(AlterarSenhaRequest request);
    Task<AlterarStatusResponse> AlterarStatus(Guid id);

    #region GraphQl

    //Task<IDictionary<Guid, UsuarioDtos>> BuscarPorIdsAsync(IEnumerable<Guid> ids);

    #endregion
}
