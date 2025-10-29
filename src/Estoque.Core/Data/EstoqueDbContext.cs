using Estoque.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace Estoque.Core.Data;
public class EstoqueDbContext : DbContext
{
    public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) : base(options) { }
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Movimentacao> Movimentacoes => Set<Movimentacao>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Produto>().HasIndex(p => p.Sku).IsUnique();
        base.OnModelCreating(modelBuilder);
    }
}
