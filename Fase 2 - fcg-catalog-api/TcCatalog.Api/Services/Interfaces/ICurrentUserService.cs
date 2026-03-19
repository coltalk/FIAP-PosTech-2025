namespace TcCatalog.API.Services.Interfaces;

public interface ICurrentUser
{
    Guid? UserId { get; }
    string Email { get; }
    string Name { get; }
    string Roles { get; }
    bool IsAuthenticated { get; }

    public bool IsAdmin()
    {
        return IsAuthenticated
            && UserId.HasValue
            && !string.IsNullOrWhiteSpace(Roles)
            && Roles.Equals("admin", StringComparison.OrdinalIgnoreCase);
    }

}