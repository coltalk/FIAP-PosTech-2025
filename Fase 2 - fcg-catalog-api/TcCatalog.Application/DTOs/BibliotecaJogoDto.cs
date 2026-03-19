namespace TcCatalog.Application.DTOs;

public class BibliotecaJogoDto : EntidadeBaseDto
{
    public string Descricao { get; set; } = default!;
    public string Genero { get; set; } = default!;
    public decimal Preco { get; set; }
    public DateTime DataCadastro { get; set; }
    public int Status { get; set; }
    public string StatusDescricao { get; set; } = default!;
    public Guid? UserId { get; set; }
    public Guid? JogoId { get; set; }
    public Guid? PayId { get; set; }
}
