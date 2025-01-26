using Xunit;
using Microsoft.EntityFrameworkCore;
using PrateleiraInteligente.Domain.Entities;
using PrateleiraInteligente.Infrastructure.Services;
using PrateleiraInteligente.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace PrateleiraInteligente.Tests.Services
{
    public class ProdutoServiceTests
    {
        private DbContextOptions<AppDbContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Theory]
        [InlineData("Produto A", 10.00, 5)]
        [InlineData("Produto B", 20.50, 10)]
        public async Task CreateAsync_ValidProduto_ReturnsProduto(string nome, decimal preco, int quantidade)
        {
            // Arrange
            using var context = new AppDbContext(CreateNewContextOptions());
            var service = new ProdutoService(context);
            var produto = new Produto
            {
                Nome = nome,
                Preco = preco,
                QuantidadeEstoque = quantidade
            };

            // Act
            var result = await service.CreateAsync(produto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nome, result.Nome);
            Assert.Equal(preco, result.Preco);
            Assert.Equal(quantidade, result.QuantidadeEstoque);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingProduto_ReturnsProduto()
        {
            // Arrange
            using var context = new AppDbContext(CreateNewContextOptions());
            var service = new ProdutoService(context);
            var produto = new Produto
            {
                Nome = "Teste Produto",
                Preco = 10.00m
            };
            context.Produtos.Add(produto);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetByIdAsync(produto.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(produto.Nome, result.Nome);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingProduto_ThrowsKeyNotFoundException()
        {
            // Arrange
            using var context = new AppDbContext(CreateNewContextOptions());
            var service = new ProdutoService(context);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                service.GetByIdAsync(999));
        }

        [Theory]
        [InlineData("Produto Original", "Produto Atualizado")]
        [InlineData("Teste A", "Teste B")]
        public async Task UpdateAsync_ExistingProduto_UpdatesSuccessfully(string nomeOriginal, string nomeAtualizado)
        {
            // Arrange
            using var context = new AppDbContext(CreateNewContextOptions());
            var service = new ProdutoService(context);
            var produto = new Produto
            {
                Nome = nomeOriginal,
                Preco = 10.00m
            };
            context.Produtos.Add(produto);
            await context.SaveChangesAsync();

            // Act
            produto.Nome = nomeAtualizado;
            await service.UpdateAsync(produto);

            // Assert
            var updatedProduto = await context.Produtos.FindAsync(produto.Id);
            Assert.NotNull(updatedProduto);
            Assert.Equal(nomeAtualizado, updatedProduto.Nome);
        }

        [Fact]
        public async Task DeleteAsync_ExistingProduto_RemovesFromDatabase()
        {
            // Arrange
            using var context = new AppDbContext(CreateNewContextOptions());
            var service = new ProdutoService(context);
            var produto = new Produto { Nome = "Produto para Deletar" };
            context.Produtos.Add(produto);
            await context.SaveChangesAsync();

            // Act
            await service.DeleteAsync(produto.Id);

            // Assert
            var deletedProduto = await context.Produtos.FindAsync(produto.Id);
            Assert.Null(deletedProduto);
        }

        [Fact]
        public async Task GetAllAsync_WithProducts_ReturnsAllProducts()
        {
            // Arrange
            using var context = new AppDbContext(CreateNewContextOptions());
            var service = new ProdutoService(context);
            var produtos = new[]
            {
                new Produto { Nome = "Produto 1" },
                new Produto { Nome = "Produto 2" },
                new Produto { Nome = "Produto 3" }
            };
            context.Produtos.AddRange(produtos);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetAllAsync();

            // Assert
            var produtosList = result.ToList();
            Assert.Equal(3, produtosList.Count);
            Assert.Contains(produtosList, p => p.Nome == "Produto 1");
            Assert.Contains(produtosList, p => p.Nome == "Produto 2");
            Assert.Contains(produtosList, p => p.Nome == "Produto 3");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ExistsAsync_ChecksExistenceCorrectly(bool shouldExist)
        {
            // Arrange
            using var context = new AppDbContext(CreateNewContextOptions());
            var service = new ProdutoService(context);
            var produto = new Produto { Nome = "Teste" };

            if (shouldExist)
            {
                context.Produtos.Add(produto);
                await context.SaveChangesAsync();
            }

            // Act
            var exists = await service.ExistsAsync(shouldExist ? produto.Id : 999);

            // Assert
            Assert.Equal(shouldExist, exists);
        }
        [Theory]
        [InlineData(5, 2)]  // 2 produtos devem vencer em 5 dias
        public async Task GetProdutosProximosVencimentoAsync_RetornaProdutosCorretos(int diasParaVencimento, int quantidadeEsperada)
        {
            // Arrange
            using var context = new AppDbContext(CreateNewContextOptions());
            var service = new ProdutoService(context);
            var dataAtual = DateTime.Now;
            var produtos = new[]
            {
        new Produto { Nome = "Produto 1", DataValidade = dataAtual.AddDays(3) },  // Deve contar
        new Produto { Nome = "Produto 2", DataValidade = dataAtual.AddDays(4) },  // Deve contar
        new Produto { Nome = "Produto 3", DataValidade = dataAtual.AddDays(10) }  // Não deve contar
    };

            context.Produtos.AddRange(produtos);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetProdutosProximosVencimentoAsync(diasParaVencimento);

            // Assert
            Assert.Equal(quantidadeEsperada, result.Count());
        }

        [Theory]
        [InlineData(5, 2)]  // Espera 2 produtos com estoque abaixo de 5
        public async Task GetProdutosBaixoEstoqueAsync_RetornaProdutosCorretos(int quantidadeMinima, int quantidadeEsperada)
        {
            // Arrange
            using var context = new AppDbContext(CreateNewContextOptions());
            var service = new ProdutoService(context);
            var produtos = new[]
            {
        new Produto { Nome = "Produto 1", QuantidadeEstoque = 2 },  // Abaixo do limite
        new Produto { Nome = "Produto 2", QuantidadeEstoque = 4 },  // Abaixo do limite
        new Produto { Nome = "Produto 3", QuantidadeEstoque = 10 }  // Acima do limite
    };

            context.Produtos.AddRange(produtos);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetProdutosBaixoEstoqueAsync(quantidadeMinima);

            // Assert
            Assert.Equal(quantidadeEsperada, result.Count());
        }
    }
}