using ControleGastos.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControleGastos.Api.Controllers;

/// <summary>
/// Controller responsável pela disponibilização de relatórios e consolidação de dados financeiros do sistema.
/// Mantém o padrão de "Thin Controller", expondo os dados que já foram processados e otimizados pela camada de Serviço.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RelatoriosController : ControllerBase
{
    private readonly IRelatorioService _relatorioService;

    public RelatoriosController(IRelatorioService relatorioService)
    {
        _relatorioService = relatorioService;
    }

    /// <summary>
    /// Obtém o relatório financeiro detalhado, agrupando o total de receitas, despesas e o saldo por pessoa.
    /// Retorna também um consolidado geral (TotalGeral) com o somatório de toda a base, conforme exigido no requisito.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelamento seguro da requisição em caso de desconexão do cliente.</param>
    /// <returns>Objeto estruturado contendo o detalhamento por pessoa e os totais gerais com status HTTP 200 (OK).</returns>
    [HttpGet("totais-por-pessoa")]
    public async Task<IActionResult> GetTotaisPorPessoa(CancellationToken cancellationToken)
    {
        // A camada de serviço já devolve o DTO perfeitamente formatado e com os cálculos resolvidos pelo EF Core.
        var result = await _relatorioService.ObterTotaisPorPessoaAsync(cancellationToken);

        return Ok(result);
    }
}