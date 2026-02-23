using ControleGastos.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace ControleGastos.Api.Middlewares;

/// <summary>
/// Middleware global para interceptação e tratamento centralizado de exceções.
/// Esta abordagem arquitetural limpa a camada de Controllers, eliminando a necessidade de blocos try/catch repetitivos,
/// e garante que os erros sejam devolvidos ao Front-end em um formato JSON padronizado.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Permite que a requisição siga o fluxo normal pelo pipeline do ASP.NET Core
            await _next(context);
        }
        catch (Exception ex)
        {
            // Intercepta qualquer exceção estourada nas camadas inferiores (Services, Domain, Infra)
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Aplica pattern matching para definir o HTTP Status Code.
        // Violações do Domínio (Regras de Negócio) mapeiam para 400 (Bad Request).
        // Qualquer outro erro inesperado (banco fora, null reference, etc) cai no fallback 500.
        context.Response.StatusCode = exception switch
        {
            RegraNegocioException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var response = new
        {
            erro = exception is RegraNegocioException ? "Erro de Validação" : "Erro Interno do Servidor",
            mensagem = exception.Message
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}