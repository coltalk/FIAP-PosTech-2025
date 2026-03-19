using TcCatalog.Domain.Enums;

namespace TcCatalog.Domain.Entities;

public class BibliotecaJogo : EntidadeBase
{
    public Guid UserId { get; set; }    
    public Guid JogoId { get; set; }
    public Guid? PayId { get; set; }
    public StatusBibliotecaJogo Status { get; set; } = StatusBibliotecaJogo.EmAberto;
    public Jogo Jogo { get; set; } = default!;
}
