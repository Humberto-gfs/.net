using Estoque.Core.Domain.Enums;
namespace Estoque.Core.Domain.Entities;
public class Produto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Sku { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public Categoria Categoria { get; set; }
    public decimal PrecoUnitario { get; set; }
    public int QuantidadeMinima { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public int Saldo { get; set; } = 0;
    public string? LotePadrao { get; set; }
    public DateTime? ValidadePadrao { get; set; }
}
