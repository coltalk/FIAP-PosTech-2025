using System.ComponentModel.DataAnnotations;

namespace TcUser.Application.DTOs.Auth;

public class RegisterRequest
{
    [Required(ErrorMessage = "Nome Ã© obrigatÃ³rio.")]
    public string Name { get; set; } = default!;

    [Required(ErrorMessage = "E-mail Ã© obrigatÃ³rio.")]
    [EmailAddress(ErrorMessage = "E-mail invÃ¡lido.")]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "Senha Ã© obrigatÃ³ria.")]
    [MinLength(8, ErrorMessage = "Senha deve ter no mÃ­nimo 8 caracteres.")]
    public string Password { get; set; } = default!;
}
