# Prova .NET – Gestão de Estoque
API em .NET 9 com EF Core InMemory implementando regras de produto, movimentação e relatórios.

## Tecnologias
- .NET 9 / ASP.NET Core Minimal APIs
- Entity Framework Core 9 (InMemory)
- xUnit + FluentAssertions
- Swagger/OpenAPI

## Regras de Negócio
- Produtos: SKU (único), Nome, Categoria (PERECIVEL=1, NAO_PERECIVEL=2), Preço Unitário, Quantidade Mínima, Data de Criação, Saldo, Lote/Validade padrão (para perecíveis).
- Perecíveis exigem lote e data de validade (cadastro e movimentações).
- Quantidade de movimentação deve ser positiva.
- Saída exige estoque suficiente.
- Relatórios:
  - Valor total do estoque = Σ (preço × saldo)
  - Produtos com validade em até 7 dias
  - Produtos com saldo abaixo do mínimo

## Diagrama (texto)
Produto (1) — (N) Movimentacao

Produto:
  Id, Sku*, Nome, Categoria, PrecoUnitario, QuantidadeMinima,
  DataCriacao, Saldo, LotePadrao?, ValidadePadrao?

Movimentacao:
  Id, ProdutoId, Tipo(ENTRADA|SAIDA), Quantidade, DataMovimentacao,
  Lote?, DataValidade?

*Sku é único.

## Como executar
1) Restaurar e compilar:
   - dotnet restore
   - dotnet build ProvaEstoque.sln

2) Rodar em Development (habilita Swagger):
   - PowerShell:
     $env:ASPNETCORE_ENVIRONMENT="Development"
     dotnet run --project src/Estoque.Api

3) Abrir Swagger:
   - http://localhost:5000/swagger

## Endpoints
- POST /api/produtos
- GET  /api/produtos/abaixo-minimo
- POST /api/movimentacoes/entrada
- POST /api/movimentacoes/saida
- GET  /api/relatorios/valor-total
- GET  /api/relatorios/vencendo
- GET  /api/relatorios/estoque-baixo

## Testes
- dotnet test ProvaEstoque.sln

## Observações
- O banco InMemory é volátil: ao parar a API, os dados somem (adequado para a prova).
- Evidências: prints do Swagger (201/400/200) e/ou arquivos em docs/evidencias/.
