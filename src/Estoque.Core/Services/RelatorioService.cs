using Estoque.Core.Data;
using Microsoft.EntityFrameworkCore;
namespace Estoque.Core.Services;
public interface IRelatorioService
{
    Task<decimal> ValorTotalEstoqueAsync(CancellationToken ct = default);
    Task<List<(Guid id,string nome,DateTime? validade)>> VencendoEm7DiasAsync(CancellationToken ct = default);
    Task<List<(Guid id,string nome,int saldo,int minimo)>> AbaixoDoMinimoAsync(CancellationToken ct = default);
}
public class RelatorioService : IRelatorioService
{
    private readonly EstoqueDbContext _db;
    public RelatorioService(EstoqueDbContext db) => _db = db;
    public async Task<decimal> ValorTotalEstoqueAsync(CancellationToken ct = default) => await _db.Produtos.SumAsync(p => p.PrecoUnitario * p.Saldo, ct);
    public async Task<List<(Guid id,string nome,DateTime? validade)>> VencendoEm7DiasAsync(CancellationToken ct = default)
    {
        var limite = DateTime.UtcNow.Date.AddDays(7);
        return await _db.Produtos.Where(p => p.ValidadePadrao.HasValue && p.ValidadePadrao.Value.Date <= limite).Select(p => new ValueTuple<Guid,string,DateTime?>(p.Id,p.Nome,p.ValidadePadrao)).ToListAsync(ct);
    }
    public async Task<List<(Guid id,string nome,int saldo,int minimo)>> AbaixoDoMinimoAsync(CancellationToken ct = default)
    {
        return await _db.Produtos.Where(p => p.Saldo < p.QuantidadeMinima).Select(p => new ValueTuple<Guid,string,int,int>(p.Id,p.Nome,p.Saldo,p.QuantidadeMinima)).ToListAsync(ct);
    }
}
