using Microsoft.Extensions.Logging;
using TcUser.Application.DTOs.Auth;
using TcUser.Application.Events;
using TcUser.Application.Interfaces;
using TcUser.Application.Validation;
using TcUser.Domain.Entities;
using TcUser.Domain.Enums;

namespace TcUser.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly ITokenService _tokens;
    private readonly IUserCreatedEventPublisher _userCreatedEventPublisher;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository users,
        ITokenService tokens,
        IUserCreatedEventPublisher userCreatedEventPublisher,
        ILogger<AuthService> logger)
    {
        _users = users;
        _tokens = tokens;
        _userCreatedEventPublisher = userCreatedEventPublisher;
        _logger = logger;
    }

    public async Task RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        var name = request.Name.Trim();
        var email = request.Email.Trim().ToLowerInvariant();
        var password = request.Password;

        AuthValidation.ValidateStrongPassword(password);

        var existing = await _users.GetByEmailAsync(email, ct);
        if (existing is not null)
            throw new ArgumentException("E-mail jÃ¡ cadastrado.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User(name, email, passwordHash, UserRole.User);

        await _users.AddAsync(user, ct);
        await _userCreatedEventPublisher.PublishAsync(
            new UserCreatedEvent(
                user.Id,
                user.Name,
                user.Email,
                user.Role.ToString(),
                user.CreatedAtUtc),
            ct);

        _logger.LogInformation("UsuÃ¡rio cadastrado com sucesso. Email={Email}", email);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        var email = request.Email.Trim().ToLowerInvariant();
        var password = request.Password;

        var user = await _users.GetByEmailAsync(email, ct);
        if (user is null)
        {
            _logger.LogWarning("Login invÃ¡lido (usuÃ¡rio nÃ£o encontrado). Email={Email}", email);
            throw new ArgumentException("Credenciais invÃ¡lidas.");
        }

        var ok = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!ok)
        {
            _logger.LogWarning("Login invÃ¡lido (senha incorreta). Email={Email}", email);
            throw new ArgumentException("Credenciais invÃ¡lidas.");
        }

        _logger.LogInformation("Login realizado com sucesso. Email={Email} Role={Role}", email, user.Role);

        return _tokens.Generate(user);
    }
}
