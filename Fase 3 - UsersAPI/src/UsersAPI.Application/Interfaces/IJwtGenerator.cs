using UsersApi.Domain.Entities;

namespace UsersAPI.Application.Interfaces;

public interface IJwtGenerator
{
    string GenerateToken(Usuario usuario);
}
