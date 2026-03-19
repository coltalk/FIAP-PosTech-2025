using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TcCatalog.API.Services.Interfaces;

[Authorize]
[ApiController]
[Route("api/UserId")]
public class TestUserId : ControllerBase
{
    private readonly ICurrentUser _currentUser;

    public TestUserId(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            _currentUser.UserId,
            _currentUser.Email,
            _currentUser.Name,
            _currentUser.Roles,
            _currentUser.IsAuthenticated
        });
    }
}