using System.ComponentModel.DataAnnotations;

namespace TcCatalog.Application.DTOs;

public class AtribuirJogoBibliotecaDto
{
    [Required]
    public Guid JogoId { get; set; }
}
