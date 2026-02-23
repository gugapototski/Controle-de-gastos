using ControleGastos.Domain.Enums;

namespace ControleGastos.Domain.Entities;

public class Categoria
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Descricao { get; set; } = string.Empty;
    public FinalidadeCategoria Finalidade { get; set; }
}