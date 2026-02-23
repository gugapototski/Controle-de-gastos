using ControleGastos.Application.Dtos;

namespace ControleGastos.Application.Interfaces;

public interface IDocumentoService
{
    Task<DocumentoResponseDto> CreateAsync(CreateDocumentoDto dto, CancellationToken cancellationToken = default);
    Task<IEnumerable<DocumentoResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
}