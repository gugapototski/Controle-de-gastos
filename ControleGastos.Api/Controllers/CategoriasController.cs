using ControleGastos.Application.Dtos;
using ControleGastos.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControleGastos.Api.Controllers;

/// <summary>
/// Controller responsável pelo gerenciamento de Categorias de transações.
/// Segue o princípio de "Thin Controller", mantendo-se enxuto e delegando 
/// a persistência e orquestração para a camada de Application.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaService _categoriaService;

    public CategoriasController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    /// <summary>
    /// Cadastra uma nova categoria no sistema, definindo sua finalidade (Receita, Despesa ou Ambas).
    /// </summary>
    /// <param name="dto">Payload contendo a descrição e a finalidade da categoria.</param>
    /// <param name="cancellationToken">Token para cancelamento da requisição assíncrona.</param>
    /// <returns>Retorna a categoria criada com o status HTTP 201 (Created).</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoriaDto dto, CancellationToken cancellationToken)
    {
        var result = await _categoriaService.CreateAsync(dto, cancellationToken);

        // Retorna 201 Created referenciando a rota do recurso criado, 
        // cumprindo as boas práticas de design de APIs RESTful.
        return Created($"/api/categorias/{result.Id}", result);
    }

    /// <summary>
    /// Lista todas as categorias disponíveis para serem vinculadas às transações.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelamento da requisição assíncrona.</param>
    /// <returns>Lista de categorias com o status HTTP 200 (OK).</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _categoriaService.GetAllAsync(cancellationToken);
        return Ok(result);
    }
}