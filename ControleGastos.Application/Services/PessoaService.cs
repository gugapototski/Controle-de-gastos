using ControleGastos.Application.Dtos;
using ControleGastos.Application.Interfaces;
using ControleGastos.Domain.Entities;
using ControleGastos.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Application.Services;

/// <summary>
/// Serviço responsável pela gestão do cadastro de Pessoas, validando dados 
/// e orquestrando a persistência junto ao banco de dados.
/// </summary>
public class PessoaService : IPessoaService
{
    private readonly IAppDbContext _context;

    public PessoaService(IAppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Cadastra uma nova pessoa no sistema.
    /// </summary>
    public async Task<PessoaResponseDto> CreateAsync(CreatePessoaDto dto, CancellationToken cancellationToken = default)
    {
        var pessoa = new Pessoa
        {
            Nome = dto.Nome,
            Idade = dto.Idade
        };

        _context.Pessoas.Add(pessoa);
        await _context.SaveChangesAsync(cancellationToken);

        return new PessoaResponseDto { Id = pessoa.Id, Nome = pessoa.Nome, Idade = pessoa.Idade };
    }

    /// <summary>
    /// Lista todas as pessoas cadastradas otimizando a query com projeção de dados (.Select).
    /// </summary>
    public async Task<IEnumerable<PessoaResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Pessoas.Select(p => new PessoaResponseDto { Id = p.Id, Nome = p.Nome, Idade = p.Idade }).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Busca uma pessoa específica pelo seu Identificador (Guid).
    /// </summary>
    public async Task<PessoaResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pessoa = await _context.Pessoas.FindAsync(new object[] { id }, cancellationToken);

        if (pessoa == null)
        {
            return null;
        }

        return new PessoaResponseDto { Id = pessoa.Id, Nome = pessoa.Nome, Idade = pessoa.Idade };
    }

    /// <summary>
    /// Exclui uma pessoa da base de dados.
    /// </summary>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pessoa = await _context.Pessoas.FindAsync(new object[] { id }, cancellationToken);

        if (pessoa == null)
        {
            throw new RegraNegocioException("Pessoa não encontrada.");
        }

        _context.Pessoas.Remove(pessoa);

        // REGRA DE NEGÓCIO DO TESTE: Em casos que se delete uma pessoa, 
        // todas as transações dessa pessoa deverão ser apagadas.
        // O código abaixo executa essa exclusão automaticamente através 
        // da configuração de Deleção em Cascata (Cascade Delete) mapeada no AppDbContext.
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Atualiza os dados cadastrais validando regras de consistência dos campos.
    /// </summary>
    public async Task UpdateAsync(Guid id, UpdatePessoaDto dto, CancellationToken cancellationToken = default)
    {
        var pessoa = await _context.Pessoas.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new RegraNegocioException("Pessoa não encontrada.");

        // Validações defensivas para garantir a integridade dos dados
        if (string.IsNullOrWhiteSpace(dto.Nome))
        {
            throw new RegraNegocioException("O nome é obrigatório.");
        }

        if (dto.Idade < 0)
        {
            throw new RegraNegocioException("A idade não pode ser negativa.");
        }

        // Atualiza as propriedades da entidade rastreada pelo Entity Framework
        pessoa.Nome = dto.Nome;
        pessoa.Idade = dto.Idade;

        await _context.SaveChangesAsync(cancellationToken);
    }
}