using TcCatalog.API.Services.Interfaces;
using System.Security.Claims;

namespace TcCatalog.API.Services;
public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal User =>
        _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;

    public Guid? UserId =>
        IsAuthenticated
            ? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))
            : null;

    public string Email =>
        User?.FindFirstValue(ClaimTypes.Email);

    //public string Name =>
    //    User?.FindFirstValue(ClaimTypes.Name);


    public string Name =>
        User?.FindFirstValue("name");

    public string Roles =>
        User?.FindFirstValue(ClaimTypes.Role);
        
}