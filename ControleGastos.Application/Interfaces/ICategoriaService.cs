using ControleGastos.Application.Dtos;

namespace ControleGastos.Application.Interfaces;

public interface ICategoriaService
{
    Task<CategoriaResponseDto> CreateAsync(CreateCategoriaDto dto, CancellationToken cancellationToken = default);
    Task<IEnumerable<CategoriaResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
}