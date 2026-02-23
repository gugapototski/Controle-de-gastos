using ControleGastos.Application.Dtos;
using ControleGastos.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControleGastos.Api.Controllers;

/// <summary>
/// Controller responsável pelos endpoints de Transações Financeiras (Documentos).
/// Mantém-se intencionalmente "magro" (Thin Controller), delegando as validações 
/// e regras de negócio exclusivamente para a camada de Application.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TransacoesController : ControllerBase
{
    private readonly IDocumentoService _documentoService;

    public TransacoesController(IDocumentoService documentoService)
    {
        _documentoService = documentoService;
    }

    /// <summary>
    /// Registra uma nova transação financeira no sistema.
    /// Aciona validações de negócio do domínio, como: compatibilidade de finalidade da categoria 
    /// e bloqueio de receitas para menores de 18 anos.
    /// </summary>
    /// <param name="dto">Objeto (payload) contendo os dados da transação.</param>
    /// <param name="cancellationToken">Token para cancelamento seguro da requisição.</param>
    /// <returns>Retorna a transação recém-criada com o status HTTP 201 (Created).</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDocumentoDto dto, CancellationToken cancellationToken)
    {
        // A lógica de negócio está blindada dentro do serviço. O controller apenas orquestra a requisição.
        var result = await _documentoService.CreateAsync(dto, cancellationToken);

        // Retorna 201 Created indicando o recurso gerado e a rota (URI) para acessá-lo, 
        // respeitando o nível de maturidade de Richardson para APIs REST.
        return Created($"/api/transacoes/{result.Id}", result);
    }

    /// <summary>
    /// Retorna o histórico completo de todas as transações cadastradas.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelamento seguro da requisição.</param>
    /// <returns>Lista de transações com status HTTP 200 (OK).</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _documentoService.GetAllAsync(cancellationToken);
        return Ok(result);
    }
}