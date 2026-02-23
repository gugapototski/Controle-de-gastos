using ControleGastos.Application.Dtos;
using ControleGastos.Application.Interfaces;
using ControleGastos.Domain.Entities;
using ControleGastos.Domain.Enums;
using ControleGastos.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Application.Services;

/// <summary>
/// Serviço central que orquestra a criação e listagem de transações (documentos), 
/// aplicando rigorosamente as regras de negócio do domínio.
/// </summary>
public class DocumentoService : IDocumentoService
{
    private readonly IAppDbContext _context;

    public DocumentoService(IAppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Cria uma nova transação após validar a existência de Pessoa e Categoria, 
    /// e garantir que todas as regras financeiras sejam atendidas.
    /// </summary>
    public async Task<DocumentoResponseDto> CreateAsync(CreateDocumentoDto documentoCreate, CancellationToken cancellationToken = default)
    {
        // Utiliza o FindAsync por questões de performance: ele verifica o tracking na memória 
        // do Entity Framework antes de realizar uma nova consulta no banco de dados.
        var pessoa = await _context.Pessoas.FindAsync(new object[] { documentoCreate.PessoaId }, cancellationToken) ?? throw new RegraNegocioException("Pessoa não encontrada.");

        var categoria = await _context.Categorias.FindAsync(new object[] { documentoCreate.CategoriaId }, cancellationToken) ?? throw new RegraNegocioException("Categoria não encontrada.");

        // O fluxo principal fica limpo (Clean Code), delegando as validações 
        // para um método privado especialista (Single Responsibility Principle).
        ValidarRegrasDeNegocio(documentoCreate, pessoa, categoria);

        var documento = new Documento
        {
            Descricao = documentoCreate.Descricao,
            Valor = documentoCreate.Valor,
            Tipo = documentoCreate.Tipo,
            CategoriaId = documentoCreate.CategoriaId,
            PessoaId = documentoCreate.PessoaId
        };

        _context.Documentos.Add(documento);
        await _context.SaveChangesAsync(cancellationToken);

        //Mapeando os dados de volta para o DTO, incluindo o ID gerado pelo banco
        return new DocumentoResponseDto
        {
            Id = documento.Id,
            Descricao = documento.Descricao,
            Valor = documento.Valor,
            Tipo = documento.Tipo,
            CategoriaId = documento.CategoriaId,
            PessoaId = documento.PessoaId
        };
    }

    /// <summary>
    /// Centraliza todas as validações de regras de negócio exigidas pelo sistema.
    /// </summary>
    private static void ValidarRegrasDeNegocio(CreateDocumentoDto documentoCreate, Pessoa pessoa, Categoria categoria)
    {
        if (documentoCreate.Valor <= 0)
        {
            throw new RegraNegocioException("O valor do documento deve ser maior que zero.");
        }

        // REGRA DE NEGÓCIO: Caso o usuário informe um menor de idade, 
        // apenas transações do tipo 'Despesa' deverão ser aceitas.
        if (pessoa.Idade < 18 && documentoCreate.Tipo == TipoTransacao.Receita)
        {
            throw new RegraNegocioException("Menores de 18 anos só podem registrar despesas.");
        }

        // REGRA DE NEGÓCIO: Restringe a utilização de categorias conforme a finalidade.
        // Exemplo: Transação de Despesa não pode usar Categoria exclusiva de Receita.
        if (documentoCreate.Tipo == TipoTransacao.Despesa && categoria.Finalidade == FinalidadeCategoria.Receita)
        {
            throw new RegraNegocioException("Não é possível usar uma categoria de Receita para uma Despesa.");
        }

        // Exemplo: Transação de Receita não pode usar Categoria exclusiva de Despesa.
        if (documentoCreate.Tipo == TipoTransacao.Receita && categoria.Finalidade == FinalidadeCategoria.Despesa)
        {
            throw new RegraNegocioException("Não é possível usar uma categoria de Despesa para uma Receita.");
        }
    }

    /// <summary>
    /// Retorna a lista de todas as transações cadastradas no sistema.
    /// </summary>
    public async Task<IEnumerable<DocumentoResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // O uso do .Select() projeta a query diretamente para o DTO, melhorando a performance e reduzindo o tráfego de dados.
        return await _context.Documentos
            .Select(d => new DocumentoResponseDto
            {
                Id = d.Id,
                Descricao = d.Descricao,
                Valor = d.Valor,
                Tipo = d.Tipo,
                CategoriaId = d.CategoriaId,
                PessoaId = d.PessoaId
            })
            .ToListAsync(cancellationToken);
    }
}