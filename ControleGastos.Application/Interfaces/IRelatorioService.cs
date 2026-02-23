using ControleGastos.Application.Dtos;

namespace ControleGastos.Application.Interfaces;

public interface IRelatorioService
{
    Task<RelatorioTotaisResponseDto> ObterTotaisPorPessoaAsync(CancellationToken cancellationToken = default);
}