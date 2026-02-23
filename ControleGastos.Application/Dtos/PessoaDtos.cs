namespace ControleGastos.Application.Dtos;

public class CreatePessoaDto
{
    public string Nome { get; set; } = string.Empty;
    public int Idade { get; set; }
}

public class PessoaResponseDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Idade { get; set; }
}

public class UpdatePessoaDto
{
    public string Nome { get; set; } = string.Empty;
    public int Idade { get; set; }
}