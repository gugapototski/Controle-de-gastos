using Moq;
using Xunit;
using ControleGastos.Application.Services;
using ControleGastos.Application.Interfaces;
using ControleGastos.Domain.Entities;
using ControleGastos.Domain.Enums;
using ControleGastos.Domain.Exceptions;
using ControleGastos.Application.Dtos;

public class DocumentoServiceTests
{
    private readonly Mock<IAppDbContext> _contextMock;
    private readonly DocumentoService _service;

    public DocumentoServiceTests()
    {
        _contextMock = new Mock<IAppDbContext>();
        _service = new DocumentoService(_contextMock.Object);
    }

    [Fact]
    public async Task CreateAsync_DeveLancarExcecao_QuandoValorForMenorOuIgualZero()
    {
        // Arrange
        var pessoaId = Guid.NewGuid();
        var categoriaId = Guid.NewGuid();

        // Precisamos simular que a Pessoa e a Categoria existem para o código passar 
        // das duas primeiras linhas do serviço e chegar na validação de valor.
        _contextMock.Setup(c => c.Pessoas.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Pessoa { Id = pessoaId, Idade = 25 });

        _contextMock.Setup(c => c.Categorias.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Categoria { Id = categoriaId, Finalidade = FinalidadeCategoria.Despesa });

        var dto = new CreateDocumentoDto
        {
            Valor = 0, // Valor inválido
            PessoaId = pessoaId,
            CategoriaId = categoriaId,
            Tipo = TipoTransacao.Despesa
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RegraNegocioException>(() => _service.CreateAsync(dto));
        Assert.Contains("maior que zero", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_DeveLancarExcecao_QuandoMenorRegistrarReceita()
    {
        // Arrange
        var pessoaId = Guid.NewGuid();
        var categoriaId = Guid.NewGuid();

        var pessoa = new Pessoa { Id = pessoaId, Idade = 16 }; // Menor de idade
        var categoria = new Categoria { Id = categoriaId, Finalidade = FinalidadeCategoria.Ambas };

        // Ajustado para dar "Match" na assinatura exata do nosso serviço (object[] + CancellationToken)
        _contextMock.Setup(c => c.Pessoas.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        _contextMock.Setup(c => c.Categorias.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoria);

        var dto = new CreateDocumentoDto
        {
            Valor = 100,
            PessoaId = pessoaId,
            CategoriaId = categoriaId,
            Tipo = TipoTransacao.Receita // Menor tentando registrar receita
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RegraNegocioException>(() => _service.CreateAsync(dto));
        Assert.Contains("Menores de 18 anos", ex.Message);
    }
}