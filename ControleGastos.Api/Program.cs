using ControleGastos.Api.Middlewares;
using ControleGastos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using ControleGastos.Application.Interfaces;
using ControleGastos.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração de CORS (Cross-Origin Resource Sharing)
// Libera o Front-end (React) para fazer requisições à API sem ser bloqueado pelo navegador.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// 2. Configuração de Persistência (Entity Framework Core)
// Define o SQLite como banco de dados, utilizando a string de conexão do appsettings.json.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Injeção de Dependência (IoC - Inversion of Control)
// Cumpre o princípio "D" do SOLID (Dependency Inversion Principle).
// Utilizamos o ciclo de vida Scoped: uma instância nova por requisição HTTP, garantindo segurança entre threads.
builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<IPessoaService, PessoaService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IDocumentoService, DocumentoService>();
builder.Services.AddScoped<IRelatorioService, RelatorioService>();

// 4. Configuração de Controllers e Swagger (Documentação)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- PIPELINE DE REQUISIÇÕES (Middlewares) ---
// A ordem aqui é vital para o funcionamento seguro da aplicação.

// 5. Configuração do ambiente de Desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 6. Tratamento Global de Exceções
// Inserido logo no início do pipeline para capturar qualquer erro estourado nas camadas inferiores.
app.UseMiddleware<GlobalExceptionMiddleware>();

// 7. Configurações de Roteamento e Segurança
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();

app.Run();