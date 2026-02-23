namespace ControleGastos.Application.Dtos;

public class ItemRelatorioPessoaDto
{
    public Guid PessoaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal TotalReceitas { get; set; }
    public decimal TotalDespesas { get; set; }
    public decimal Saldo { get; set; }
}

public class TotalGeralDto
{
    public decimal TotalReceitas { get; set; }
    public decimal TotalDespesas { get; set; }
    public decimal Saldo { get; set; }
}

public class RelatorioTotaisResponseDto
{
    public IEnumerable<ItemRelatorioPessoaDto> Itens { get; set; } = new List<ItemRelatorioPessoaDto>();
    public TotalGeralDto Geral { get; set; } = new TotalGeralDto();
}