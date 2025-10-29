using Estoque.Core.Data;
using Estoque.Core.Domain.Entities;
using Estoque.Core.Domain.Enums;
using Estoque.Core.Domain.Exceptions;
using Estoque.Core.DTOs;
using Estoque.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<EstoqueDbContext>(opt => opt.UseInMemoryDatabase("estoque-db"));
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IMovimentacaoService, MovimentacaoService>();
builder.Services.AddScoped<IRelatorioService, RelatorioService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseExceptionHandler(a => a.Run(async context =>
{
    var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
    if (feature?.Error is DomainException dex) { context.Response.StatusCode = 400; await context.Response.WriteAsJsonAsync(new { error = dex.Message }); }
}));
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapPost("/api/produtos", async ([FromBody] ProdutoCreateDto dto, IProdutoService svc) =>
{
    var p = new Produto { Sku = dto.Sku, Nome = dto.Nome, Categoria = dto.Categoria, PrecoUnitario = dto.PrecoUnitario, QuantidadeMinima = dto.QuantidadeMinima, Saldo = dto.SaldoInicial, LotePadrao = dto.LotePadrao, ValidadePadrao = dto.ValidadePadrao };
    var novo = await svc.CadastrarAsync(p);
    return Results.Created($"/api/produtos/{novo.Id}", new ProdutoReadDto(novo.Id, novo.Sku, novo.Nome, novo.Categoria, novo.PrecoUnitario, novo.QuantidadeMinima, novo.Saldo, novo.DataCriacao.Year, novo.DataCriacao, novo.LotePadrao, novo.ValidadePadrao));
});
app.MapGet("/api/produtos/abaixo-minimo", async (IProdutoService svc) => Results.Ok(await svc.AbaixoDoMinimoAsync()));
app.MapPost("/api/movimentacoes/entrada", async ([FromBody] MovimentacaoCreateDto dto, IMovimentacaoService svc) =>
{
    var mov = new Movimentacao { ProdutoId = dto.ProdutoId, Tipo = TipoMovimentacao.ENTRADA, Quantidade = dto.Quantidade, DataMovimentacao = dto.DataMovimentacao ?? DateTime.UtcNow, Lote = dto.Lote, DataValidade = dto.DataValidade };
    var res = await svc.RegistrarAsync(mov);
    return Results.Created($"/api/movimentacoes/{res.Id}", res);
});
app.MapPost("/api/movimentacoes/saida", async ([FromBody] MovimentacaoCreateDto dto, IMovimentacaoService svc) =>
{
    var mov = new Movimentacao { ProdutoId = dto.ProdutoId, Tipo = TipoMovimentacao.SAIDA, Quantidade = dto.Quantidade, DataMovimentacao = dto.DataMovimentacao ?? DateTime.UtcNow, Lote = dto.Lote, DataValidade = dto.DataValidade };
    var res = await svc.RegistrarAsync(mov);
    return Results.Created($"/api/movimentacoes/{res.Id}", res);
});
app.MapGet("/api/relatorios/valor-total", async (IRelatorioService svc) => Results.Ok(new { valorTotal = await svc.ValorTotalEstoqueAsync() }));
app.MapGet("/api/relatorios/vencendo", async (IRelatorioService svc) => Results.Ok(await svc.VencendoEm7DiasAsync()));
app.MapGet("/api/relatorios/estoque-baixo", async (IRelatorioService svc) => Results.Ok(await svc.AbaixoDoMinimoAsync()));
app.Run();
