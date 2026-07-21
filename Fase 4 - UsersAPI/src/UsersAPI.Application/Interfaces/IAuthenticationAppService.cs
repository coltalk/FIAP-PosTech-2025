using UsersAPI.Domain.Dtos.Responses.Authentication;

namespace UsersAPI.Application.Interfaces;

public interface IAuthenticationAppService
{
    Task<LoginResponse> Login(string usuario, string senha);
}
