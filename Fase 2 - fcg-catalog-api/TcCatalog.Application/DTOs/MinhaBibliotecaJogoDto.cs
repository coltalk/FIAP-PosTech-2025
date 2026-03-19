namespace TcCatalog.Application.DTOs;

public class MinhaBibliotecaJogoDto
{
    public string Descricao { get; set; } = default!;
    public string Genero { get; set; } = default!;
    public decimal Preco { get; set; }
    public DateTime DataCadastro { get; set; }
}
