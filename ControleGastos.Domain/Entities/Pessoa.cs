namespace ControleGastos.Domain.Entities;

public class Pessoa
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public int Idade { get; set; }

    // Propriedade de navegação para o EF Core saber que existe a relação 1:N
    public ICollection<Documento> Documentos { get; set; } = new List<Documento>();
}