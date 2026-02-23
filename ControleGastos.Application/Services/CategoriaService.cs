using ControleGastos.Application.Dtos;
using ControleGastos.Application.Interfaces;
using ControleGastos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Application.Services;

/// <summary>
/// Serviço responsável pela orquestração e regras de negócio do cadastro de Categorias.
/// </summary>
public class CategoriaService : ICategoriaService
{
    private readonly IAppDbContext _context;

    public CategoriaService(IAppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Cria uma nova categoria no sistema.
    /// </summary>
    /// <param name="dto">Dados da categoria a ser criada.</param>
    /// <param name="cancellationToken">Token para cancelamento da operação assíncrona.</param>
    public async Task<CategoriaResponseDto> CreateAsync(CreateCategoriaDto dto, CancellationToken cancellationToken = default)
    {
        // Mapeamento simples do DTO (entrada) para a Entidade de Domínio
        var categoria = new Categoria
        {
            Descricao = dto.Descricao,
            Finalidade = dto.Finalidade
        };

        _context.Categorias.Add(categoria);

        // Persiste a entidade no banco de dados repassando o token de cancelamento
        await _context.SaveChangesAsync(cancellationToken);

        return new CategoriaResponseDto
        {
            Id = categoria.Id,
            Descricao = categoria.Descricao,
            Finalidade = categoria.Finalidade
        };
    }

    /// <summary>
    /// Retorna a lista de todas as categorias cadastradas.
    /// </summary>
    public async Task<IEnumerable<CategoriaResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Utiliza o .Select() para projetar os dados diretamente no DTO.
        // Isso otimiza a performance, pois o Entity Framework fará um SELECT apenas nas colunas necessárias,
        // evitando carregar entidades completas (tracking) desnecessariamente na memória.
        return await _context.Categorias
            .Select(c => new CategoriaResponseDto
            {
                Id = c.Id,
                Descricao = c.Descricao,
                Finalidade = c.Finalidade
            })
            .ToListAsync(cancellationToken); // Repassando o token para a query do banco
    }
}