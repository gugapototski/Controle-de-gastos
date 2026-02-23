using ControleGastos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ControleGastos.Application.Interfaces;

namespace ControleGastos.Infrastructure.Persistence;

/// <summary>
/// Contexto principal do Entity Framework Core.
/// Responsável por mapear as entidades de Domínio para as tabelas do banco de dados (SQLite)
/// utilizando a Fluent API para garantir a integridade e os limites definidos nos requisitos do negócio.
/// </summary>
public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Pessoa> Pessoas { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Documento> Documentos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // REQUISITO DO TESTE: Nome com tamanho máximo de 200.
        modelBuilder.Entity<Pessoa>().ToTable("Pessoa");
        modelBuilder.Entity<Pessoa>().HasKey(p => p.Id);
        modelBuilder.Entity<Pessoa>().Property(p => p.Nome).IsRequired().HasMaxLength(200);
        modelBuilder.Entity<Pessoa>().Property(p => p.Idade).IsRequired();

        // REQUISITO DO TESTE: Descrição da Categoria com tamanho máximo de 400.
        modelBuilder.Entity<Categoria>().ToTable("Categoria");
        modelBuilder.Entity<Categoria>().HasKey(c => c.Id);
        modelBuilder.Entity<Categoria>().Property(c => c.Descricao).IsRequired().HasMaxLength(400);
        modelBuilder.Entity<Categoria>().Property(c => c.Finalidade).IsRequired();

        // REQUISITO DO TESTE: Descrição da Transação com tamanho máximo de 400.
        modelBuilder.Entity<Documento>().ToTable("Documento");
        modelBuilder.Entity<Documento>().HasKey(d => d.Id);
        modelBuilder.Entity<Documento>().Property(d => d.Descricao).IsRequired().HasMaxLength(400);
        modelBuilder.Entity<Documento>().Property(d => d.Valor).IsRequired().HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Documento>().Property(d => d.Tipo).IsRequired();

        // REQUISITO DO TESTE: Em casos que se delete uma pessoa, todas as transações dessa pessoa deverão ser apagadas.
        // Configurado via Deleção em Cascata (Cascade) para garantir a consistência a nível de banco de dados.
        modelBuilder.Entity<Documento>()
            .HasOne(d => d.Pessoa)
            .WithMany(p => p.Documentos)
            .HasForeignKey(d => d.PessoaId)
            .OnDelete(DeleteBehavior.Cascade);

        // REGRA DE SEGURANÇA BÔNUS: Impede apagar uma categoria se houver documentos vinculados a ela (Evita dados órfãos).
        modelBuilder.Entity<Documento>()
            .HasOne(d => d.Categoria)
            .WithMany()
            .HasForeignKey(d => d.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}