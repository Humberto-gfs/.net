using Estoque.Core.Domain.Enums;
namespace Estoque.Core.DTOs;
public record MovimentacaoCreateDto(Guid ProdutoId,TipoMovimentacao Tipo,int Quantidade,DateTime? DataMovimentacao,string? Lote,DateTime? DataValidade);
