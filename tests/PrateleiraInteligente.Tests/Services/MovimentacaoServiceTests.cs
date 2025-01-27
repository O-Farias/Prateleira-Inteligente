using Xunit;
using Microsoft.EntityFrameworkCore;
using PrateleiraInteligente.Domain.Entities;
using PrateleiraInteligente.Infrastructure.Services;
using PrateleiraInteligente.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace PrateleiraInteligente.Tests.Services
{
    public class MovimentacaoServiceTests
    {
        private DbContextOptions<AppDbContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task RegistrarEntradaAsync_DeveAumentarEstoque()
        {
            // Arrange
            using var context = new AppDbContext(CreateNewContextOptions());
            var service = new MovimentacaoService(context);

            var produto = new Produto
            {
                Nome = "Produto Teste",
                QuantidadeEstoque = 10
            };
            context.Produtos.Add(produto);
            await context.SaveChangesAsync();

            // Act
            var movimentacao = await service.RegistrarEntradaAsync(
                produto.Id,
                5,
                "Entrada de teste"
            );

            // Assert
            Assert.NotNull(movimentacao);
            Assert.Equal(TipoMovimentacao.Entrada, movimentacao.Tipo);
            Assert.Equal(5, movimentacao.Quantidade);

            var produtoAtualizado = await context.Produtos.FindAsync(produto.Id);
            Assert.NotNull(produtoAtualizado);
            Assert.Equal(15, produtoAtualizado.QuantidadeEstoque);
        }

        [Fact]
        public async Task RegistrarSaidaAsync_DeveReduzirEstoque()
        {
            // Arrange
            using var context = new AppDbContext(CreateNewContextOptions());
            var service = new MovimentacaoService(context);

            var produto = new Produto
            {
                Nome = "Produto Teste",
                QuantidadeEstoque = 10
            };
            context.Produtos.Add(produto);
            await context.SaveChangesAsync();

            // Act
            var movimentacao = await service.RegistrarSaidaAsync(
                produto.Id,
                3,
                "Saída de teste"
            );

            // Assert
            Assert.NotNull(movimentacao);
            Assert.Equal(TipoMovimentacao.Saida, movimentacao.Tipo);
            Assert.Equal(3, movimentacao.Quantidade);

            var produtoAtualizado = await context.Produtos.FindAsync(produto.Id);
            Assert.NotNull(produtoAtualizado);
            Assert.Equal(7, produtoAtualizado.QuantidadeEstoque);
        }

        [Fact]
        public async Task RegistrarSaidaAsync_EstoqueInsuficiente_DeveLancarExcecao()
        {
            // Arrange
            using var context = new AppDbContext(CreateNewContextOptions());
            var service = new MovimentacaoService(context);

            var produto = new Produto
            {
                Nome = "Produto Teste",
                QuantidadeEstoque = 5
            };
            context.Produtos.Add(produto);
            await context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.RegistrarSaidaAsync(produto.Id, 10, "Saída maior que estoque"));
        }

        [Fact]
        public async Task GetMovimentacoesPorProdutoAsync_DeveRetornarMovimentacoes()
        {
            // Arrange
            using var context = new AppDbContext(CreateNewContextOptions());
            var service = new MovimentacaoService(context);

            var produto = new Produto { Nome = "Produto Teste" };
            context.Produtos.Add(produto);
            await context.SaveChangesAsync();

            await service.RegistrarEntradaAsync(produto.Id, 5, "Entrada 1");
            await service.RegistrarEntradaAsync(produto.Id, 3, "Entrada 2");
            await service.RegistrarSaidaAsync(produto.Id, 2, "Saída 1");

            // Act
            var movimentacoes = await service.GetMovimentacoesPorProdutoAsync(produto.Id);

            // Assert
            Assert.NotNull(movimentacoes);
            var lista = movimentacoes.ToList();
            Assert.Equal(3, lista.Count);
            Assert.Contains(lista, m => m.Quantidade == 5);
            Assert.Contains(lista, m => m.Quantidade == 3);
            Assert.Contains(lista, m => m.Quantidade == 2);
        }

        [Fact]
        public async Task RegistrarMovimentacao_ProdutoInexistente_DeveLancarExcecao()
        {
            // Arrange
            using var context = new AppDbContext(CreateNewContextOptions());
            var service = new MovimentacaoService(context);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                service.RegistrarEntradaAsync(999, 5, "Produto não existe"));
        }
    }
}