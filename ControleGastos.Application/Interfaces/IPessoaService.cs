using ControleGastos.Application.Dtos;

namespace ControleGastos.Application.Interfaces;

public interface IPessoaService
{
    Task<PessoaResponseDto> CreateAsync(CreatePessoaDto dto, CancellationToken cancellationToken = default);
    Task<IEnumerable<PessoaResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PessoaResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task UpdateAsync(Guid id, UpdatePessoaDto dto, CancellationToken cancellationToken = default);
}