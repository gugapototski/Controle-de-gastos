using ControleGastos.Domain.Enums;

namespace ControleGastos.Application.Dtos;

public class CreateCategoriaDto
{
    public string Descricao { get; set; } = string.Empty;
    public FinalidadeCategoria Finalidade { get; set; }
}

public class CategoriaResponseDto
{
    public Guid Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public FinalidadeCategoria Finalidade { get; set; }
}