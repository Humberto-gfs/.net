using Estoque.Core.Data;
using Estoque.Core.Domain.Entities;
using Estoque.Core.Domain.Enums;
using Estoque.Core.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace Estoque.Core.Services;
public interface IProdutoService
{
    Task<Produto> CadastrarAsync(Produto produto, CancellationToken ct = default);
    Task<List<Produto>> AbaixoDoMinimoAsync(CancellationToken ct = default);
    Task<Produto?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
}
public class ProdutoService : IProdutoService
{
    private readonly EstoqueDbContext _db;
    public ProdutoService(EstoqueDbContext db) => _db = db;
    public async Task<Produto> CadastrarAsync(Produto produto, CancellationToken ct = default)
    {
        if (produto.PrecoUnitario < 0) throw new DomainException("Preço unitário não pode ser negativo.");
        if (produto.QuantidadeMinima < 0) throw new DomainException("Quantidade mínima não pode ser negativa.");
        if (produto.Categoria == Categoria.PERECIVEL)
        {
            if (string.IsNullOrWhiteSpace(produto.LotePadrao) || !produto.ValidadePadrao.HasValue) throw new DomainException("Produtos perecíveis devem possuir lote e data de validade.");
            if (produto.ValidadePadrao.Value.Date <= DateTime.UtcNow.Date) throw new DomainException("Validade padrão para perecíveis deve ser futura.");
        }
        _db.Produtos.Add(produto);
        await _db.SaveChangesAsync(ct);
        return produto;
    }
    public async Task<List<Produto>> AbaixoDoMinimoAsync(CancellationToken ct = default) => await _db.Produtos.Where(p => p.Saldo < p.QuantidadeMinima).ToListAsync(ct);
    public Task<Produto?> ObterPorIdAsync(Guid id, CancellationToken ct = default) => _db.Produtos.FirstOrDefaultAsync(p => p.Id == id, ct);
}
