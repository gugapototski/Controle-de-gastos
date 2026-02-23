using ControleGastos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Application.Interfaces;

public interface IAppDbContext
{
    DbSet<Pessoa> Pessoas { get; }
    DbSet<Categoria> Categorias { get; }
    DbSet<Documento> Documentos { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}