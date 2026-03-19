using TcUser.Application.DTOs.Auth;
using TcUser.Domain.Entities;

namespace TcUser.Application.Interfaces;

public interface ITokenService
{
    AuthResponse Generate(User user);
}
