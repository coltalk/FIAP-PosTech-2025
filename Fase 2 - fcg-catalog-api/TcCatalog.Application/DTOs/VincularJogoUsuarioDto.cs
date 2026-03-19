using System.ComponentModel.DataAnnotations;

namespace TcCatalog.Application.DTOs;

public class VincularJogoUsuarioDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid JogoId { get; set; }
}
