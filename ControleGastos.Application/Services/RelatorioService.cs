using ControleGastos.Application.Dtos;
using ControleGastos.Application.Interfaces;
using ControleGastos.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Application.Services;

/// <summary>
/// Serviço responsável pela geração de relatórios e consolidação de dados financeiros do sistema.
/// </summary>
public class RelatorioService : IRelatorioService
{
    private readonly IAppDbContext _context;

    public RelatorioService(IAppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtém o relatório detalhado de totais (receitas, despesas e saldo) agrupado por pessoa.
    /// Também calcula o totalizador geral exigido pela regra de negócio.
    /// </summary>
    public async Task<RelatorioTotaisResponseDto> ObterTotaisPorPessoaAsync(CancellationToken cancellationToken = default)
    {
        // A consulta abaixo utiliza a execução diferida (IQueryable) do Entity Framework.
        // O LINQ traduzirá os comandos Sum() e Where() para operações nativas no SQLite,
        // garantindo que o cálculo de totais seja feito no banco de dados e não na memória da aplicação.
        var itens = await _context.Pessoas
            .Select(p => new ItemRelatorioPessoaDto
            {
                PessoaId = p.Id,
                Nome = p.Nome,
                TotalReceitas = p.Documentos.Where(d => d.Tipo == TipoTransacao.Receita).Sum(d => d.Valor),
                TotalDespesas = p.Documentos.Where(d => d.Tipo == TipoTransacao.Despesa).Sum(d => d.Valor),
                Saldo = p.Documentos.Where(d => d.Tipo == TipoTransacao.Receita).Sum(d => d.Valor) -
                        p.Documentos.Where(d => d.Tipo == TipoTransacao.Despesa).Sum(d => d.Valor)
            })
            .ToListAsync(cancellationToken);

        // O cálculo do total geral é feito em memória de forma muito rápida, 
        // pois a lista 'itens' já foi sumarizada e reduzida pelo banco de dados acima.
        var totalGeralReceitas = itens.Sum(i => i.TotalReceitas);
        var totalGeralDespesas = itens.Sum(i => i.TotalDespesas);

        return new RelatorioTotaisResponseDto
        {
            Itens = itens,
            Geral = new TotalGeralDto
            {
                TotalReceitas = totalGeralReceitas,
                TotalDespesas = totalGeralDespesas,
                Saldo = totalGeralReceitas - totalGeralDespesas
            }
        };
    }
}