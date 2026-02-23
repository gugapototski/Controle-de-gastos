namespace ControleGastos.Domain.Exceptions;

/// <summary>
/// Exceção customizada para representar violações de regras de negócio estritas do Domínio.
/// Criada para encapsular erros de validação e permitir que a camada de Apresentação (Middleware/API) 
/// consiga diferenciar uma regra de negócio quebrada (HTTP 400) de um erro inesperado do sistema (HTTP 500).
/// </summary>
public class RegraNegocioException : Exception
{
    public RegraNegocioException(string message) : base(message)
    {
    }
}