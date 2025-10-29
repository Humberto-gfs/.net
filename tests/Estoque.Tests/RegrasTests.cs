using Xunit;
using Estoque.Core.Data;
using Estoque.Core.Domain.Entities;
using Estoque.Core.Domain.Enums;
using Estoque.Core.Domain.Exceptions;
using Estoque.Core.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
namespace Estoque.Tests;
public class RegrasTests
{
    private EstoqueDbContext NewDb()
    {
        var opts = new DbContextOptionsBuilder<EstoqueDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        return new EstoqueDbContext(opts);
    }
    [Fact]
    public async Task Nao_deve_cadastrar_perecivel_sem_validade_ou_lote()
    {
        using var db = NewDb();
        var svc = new ProdutoService(db);
        var produto = new Produto { Sku = "P-001", Nome = "Iogurte", Categoria = Categoria.PERECIVEL, PrecoUnitario = 5, QuantidadeMinima = 2, Saldo = 10 };
        await FluentActions.Awaiting(() => svc.CadastrarAsync(produto)).Should().ThrowAsync<DomainException>();
    }
    [Fact]
    public async Task Nao_deve_permitir_saida_com_estoque_insuficiente()
    {
        using var db = NewDb();
        var psvc = new ProdutoService(db);
        var msvc = new MovimentacaoService(db);
        var p = await psvc.CadastrarAsync(new Produto { Sku = "N-100", Nome = "Caderno", Categoria = Categoria.NAO_PERECIVEL, PrecoUnitario = 10, QuantidadeMinima = 1, Saldo = 3 });
        var saida = new Movimentacao { ProdutoId = p.Id, Tipo = TipoMovimentacao.SAIDA, Quantidade = 5 };
        await FluentActions.Awaiting(() => msvc.RegistrarAsync(saida)).Should().ThrowAsync<DomainException>();
    }
    [Fact]
    public async Task Deve_bloquear_movimentacao_com_quantidade_negativa_ou_zero()
    {
        using var db = NewDb();
        var psvc = new ProdutoService(db);
        var msvc = new MovimentacaoService(db);
        var p = await psvc.CadastrarAsync(new Produto { Sku = "N-101", Nome = "Caneta", Categoria = Categoria.NAO_PERECIVEL, PrecoUnitario = 2, QuantidadeMinima = 5, Saldo = 10 });
        var entradaZero = new Movimentacao { ProdutoId = p.Id, Tipo = TipoMovimentacao.ENTRADA, Quantidade = 0 };
        await FluentActions.Awaiting(() => msvc.RegistrarAsync(entradaZero)).Should().ThrowAsync<DomainException>();
    }
    [Fact]
    public async Task Perecivel_nao_pode_saida_apos_validade_e_alerta_minimo()
    {
        using var db = NewDb();
        var psvc = new ProdutoService(db);
        var msvc = new MovimentacaoService(db);
        var p = await psvc.CadastrarAsync(new Produto { Sku = "P-777", Nome = "Queijo", Categoria = Categoria.PERECIVEL, PrecoUnitario = 20, QuantidadeMinima = 5, Saldo = 6, LotePadrao = "L123", ValidadePadrao = DateTime.UtcNow.AddDays(3) });
        await msvc.RegistrarAsync(new Movimentacao { ProdutoId = p.Id, Tipo = TipoMovimentacao.SAIDA, Quantidade = 2, Lote = "L123", DataValidade = p.ValidadePadrao });
        (p.Saldo < p.QuantidadeMinima).Should().BeTrue();
        var vencida = p.ValidadePadrao!.Value.AddDays(-5);
        var saida = new Movimentacao { ProdutoId = p.Id, Tipo = TipoMovimentacao.SAIDA, Quantidade = 1, Lote = "L123", DataValidade = vencida };
        await FluentActions.Awaiting(() => msvc.RegistrarAsync(saida)).Should().ThrowAsync<DomainException>();
    }
    [Fact]
    public async Task Calculo_de_saldo_e_valor_total_estoque()
    {
        using var db = NewDb();
        var psvc = new ProdutoService(db);
        var msvc = new MovimentacaoService(db);
        var rsvc = new RelatorioService(db);
        var p = await psvc.CadastrarAsync(new Produto { Sku = "N-200", Nome = "Mouse", Categoria = Categoria.NAO_PERECIVEL, PrecoUnitario = 50, QuantidadeMinima = 1, Saldo = 0 });
        await msvc.RegistrarAsync(new Movimentacao { ProdutoId = p.Id, Tipo = TipoMovimentacao.ENTRADA, Quantidade = 10 });
        await msvc.RegistrarAsync(new Movimentacao { ProdutoId = p.Id, Tipo = TipoMovimentacao.SAIDA, Quantidade = 3 });
        p.Saldo.Should().Be(7);
        var total = await rsvc.ValorTotalEstoqueAsync();
        total.Should().Be(350);
    }
}
