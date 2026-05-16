namespace UsersAPI.Domain.Dtos.Responses.Role;

public record RolesResponse
{
    public int Id { get; init; }
    public string RoleName { get; init; }
    public string? Description { get; init; }
}
