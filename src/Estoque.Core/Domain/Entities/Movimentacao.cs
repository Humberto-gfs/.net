using Estoque.Core.Domain.Enums;
namespace Estoque.Core.Domain.Entities;
public class Movimentacao
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProdutoId { get; set; }
    public TipoMovimentacao Tipo { get; set; }
    public int Quantidade { get; set; }
    public DateTime DataMovimentacao { get; set; } = DateTime.UtcNow;
    public string? Lote { get; set; }
    public DateTime? DataValidade { get; set; }
}
