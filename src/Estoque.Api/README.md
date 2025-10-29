# Prova .NET – Gestão de Estoque (.NET 9, EF InMemory)

Como executar
```
dotnet restore
dotnet build ProvaEstoque.sln
dotnet run --project src/Estoque.Api
```
Swagger: `/swagger`

Endpoints
- POST /api/produtos
- GET  /api/produtos/abaixo-minimo
- POST /api/movimentacoes/entrada
- POST /api/movimentacoes/saida
- GET  /api/relatorios/valor-total
- GET  /api/relatorios/vencendo
- GET  /api/relatorios/estoque-baixo
