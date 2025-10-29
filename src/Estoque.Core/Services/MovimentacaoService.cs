using Estoque.Core.Data;
using Estoque.Core.Domain.Entities;
using Estoque.Core.Domain.Enums;
using Estoque.Core.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace Estoque.Core.Services;
public interface IMovimentacaoService { Task<Movimentacao> RegistrarAsync(Movimentacao mov, CancellationToken ct = default); }
public class MovimentacaoService : IMovimentacaoService
{
    private readonly EstoqueDbContext _db;
    public MovimentacaoService(EstoqueDbContext db) => _db = db;
    public async Task<Movimentacao> RegistrarAsync(Movimentacao mov, CancellationToken ct = default)
    {
        if (mov.Quantidade <= 0) throw new DomainException("Quantidade deve ser positiva.");
        var prod = await _db.Produtos.FirstOrDefaultAsync(p => p.Id == mov.ProdutoId, ct) ?? throw new DomainException("Produto não encontrado.");
        if (prod.Categoria == Categoria.PERECIVEL)
        {
            if (string.IsNullOrWhiteSpace(mov.Lote) || !mov.DataValidade.HasValue) throw new DomainException("Movimentação de perecível requer lote e data de validade.");
            if (mov.Tipo == TipoMovimentacao.SAIDA && mov.DataValidade.Value.Date < DateTime.UtcNow.Date) throw new DomainException("Produto perecível não pode ter SAÍDA após a data de validade.");
            if (mov.Tipo == TipoMovimentacao.ENTRADA && mov.DataValidade.Value.Date <= DateTime.UtcNow.Date) throw new DomainException("Não é permitida ENTRADA com validade vencida.");
        }
        if (mov.Tipo == TipoMovimentacao.SAIDA)
        {
            if (prod.Saldo < mov.Quantidade) throw new DomainException("Estoque insuficiente para saída.");
            prod.Saldo -= mov.Quantidade;
        }
        else if (mov.Tipo == TipoMovimentacao.ENTRADA) { prod.Saldo += mov.Quantidade; }
        _db.Movimentacoes.Add(mov);
        await _db.SaveChangesAsync(ct);
        return mov;
    }
}
