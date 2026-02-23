using ControleGastos.Application.Dtos;
using ControleGastos.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControleGastos.Api.Controllers;

/// <summary>
/// Controller responsável pelo gerenciamento do cadastro de Pessoas.
/// Atua como uma fina camada de roteamento RESTful, delegando as regras de negócio 
/// e validações estruturais para a camada de Serviço.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PessoasController : ControllerBase
{
    private readonly IPessoaService _pessoaService;

    public PessoasController(IPessoaService pessoaService)
    {
        _pessoaService = pessoaService;
    }

    /// <summary>
    /// Cadastra uma nova pessoa no sistema.
    /// </summary>
    /// <param name="dto">Dados de entrada (Nome e Idade).</param>
    /// <param name="cancellationToken">Token para cancelamento seguro da requisição.</param>
    /// <returns>Retorna a pessoa gerada, status HTTP 201 (Created) e o cabeçalho Location com a URI de acesso.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePessoaDto dto, CancellationToken cancellationToken)
    {
        var result = await _pessoaService.CreateAsync(dto, cancellationToken);

        // Retorna o HTTP 201 referenciando dinamicamente a rota GetById, 
        // a melhor prática do padrão REST para criação de recursos.
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Retorna a lista completa de todas as pessoas cadastradas na base.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _pessoaService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Busca os detalhes de uma pessoa específica pelo seu Identificador Único (Guid).
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _pessoaService.GetByIdAsync(id, cancellationToken);

        if (result == null)
            return NotFound(); // Padrão HTTP 404 caso o recurso não exista

        return Ok(result);
    }

    /// <summary>
    /// Remove uma pessoa do sistema.
    /// REQUISITO: Em casos que se delete uma pessoa, todas as transações (Documentos) atreladas 
    /// a ela são apagadas automaticamente em cascata pela configuração de infraestrutura do Entity Framework.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        // Se a pessoa não existir, o nosso Middleware Global vai capturar a RegraNegocioException lançada no Service 
        // e devolver um HTTP 400 amigável para o Front-end, limpando o código deste controller.
        await _pessoaService.DeleteAsync(id, cancellationToken);

        return NoContent(); // Padrão HTTP 204 (No Content) indicando sucesso sem corpo de resposta
    }

    /// <summary>
    /// Atualiza os dados de uma pessoa (como correção de nome ou atualização de idade).
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePessoaDto dto, CancellationToken cancellationToken)
    {
        await _pessoaService.UpdateAsync(id, dto, cancellationToken);

        return NoContent(); // Padrão HTTP 204 para atualizações que ocorrem com sucesso
    }
}