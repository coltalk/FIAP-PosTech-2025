using System.ComponentModel.DataAnnotations;

namespace TcUser.Application.DTOs.Auth;

public class LoginRequest
{
    [Required(ErrorMessage = "E-mail Ã© obrigatÃ³rio.")]
    [EmailAddress(ErrorMessage = "E-mail invÃ¡lido.")]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "Senha Ã© obrigatÃ³ria.")]
    public string Password { get; set; } = default!;
}
