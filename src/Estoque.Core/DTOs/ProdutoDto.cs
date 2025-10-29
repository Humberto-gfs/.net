using Estoque.Core.Domain.Enums;
namespace Estoque.Core.DTOs;
public record ProdutoCreateDto(string Sku,string Nome,Categoria Categoria,decimal PrecoUnitario,int QuantidadeMinima,int SaldoInicial,string? LotePadrao,DateTime? ValidadePadrao);
public record ProdutoReadDto(Guid Id,string Sku,string Nome,Categoria Categoria,decimal PrecoUnitario,int QuantidadeMinima,int Saldo,int? AnoCriacao,DateTime DataCriacao,string? LotePadrao,DateTime? ValidadePadrao);
