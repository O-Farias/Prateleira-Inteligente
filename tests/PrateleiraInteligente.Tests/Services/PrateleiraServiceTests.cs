using Xunit;
using Microsoft.EntityFrameworkCore;
using PrateleiraInteligente.Domain.Entities;
using PrateleiraInteligente.Infrastructure.Services;
using PrateleiraInteligente.Infrastructure.Persistence;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PrateleiraInteligente.Tests.Services
{
    public class PrateleiraServiceTests
    {
        private DbContextOptions<AppDbContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task CreateAsync_ValidPrateleira_ReturnsPrateleira()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new AppDbContext(options);
            var service = new PrateleiraService(context);
            var prateleira = new Prateleira 
            { 
                Nome = "Prateleira Teste",
                Localizacao = "Setor A",
                CapacidadeMaxima = 10
            };

            // Act
            var result = await service.CreateAsync(prateleira);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Prateleira Teste", result.Nome);
            Assert.Equal("Setor A", result.Localizacao);
            Assert.Equal(10, result.CapacidadeMaxima);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingPrateleira_ReturnsPrateleira()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new AppDbContext(options);
            var service = new PrateleiraService(context);
            var prateleira = new Prateleira 
            { 
                Nome = "Prateleira Teste",
                Localizacao = "Setor A"
            };
            context.Prateleiras.Add(prateleira);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetByIdAsync(prateleira.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(prateleira.Nome, result.Nome);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingPrateleira_ReturnsNull()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new AppDbContext(options);
            var service = new PrateleiraService(context);

            // Act
            var result = await service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task TemEspacoDisponivel_ComEspaco_ReturnsTrue()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new AppDbContext(options);
            var service = new PrateleiraService(context);
            var prateleira = new Prateleira
            {
                Nome = "Prateleira Teste",
                CapacidadeMaxima = 5
            };
            context.Prateleiras.Add(prateleira);
            await context.SaveChangesAsync();

            var produto = new Produto { Nome = "Produto 1", PrateleiraId = prateleira.Id };
            context.Produtos.Add(produto);
            await context.SaveChangesAsync();

            // Act
            var result = await service.TemEspacoDisponivel(prateleira.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task TemEspacoDisponivel_SemEspaco_ReturnsFalse()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new AppDbContext(options);
            var service = new PrateleiraService(context);
            var prateleira = new Prateleira
            {
                Nome = "Prateleira Teste",
                CapacidadeMaxima = 1
            };
            context.Prateleiras.Add(prateleira);
            await context.SaveChangesAsync();

            var produto = new Produto { Nome = "Produto 1", PrateleiraId = prateleira.Id };
            context.Produtos.Add(produto);
            await context.SaveChangesAsync();

            // Act
            var result = await service.TemEspacoDisponivel(prateleira.Id);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ExistingPrateleira_RemovesFromDatabase()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new AppDbContext(options);
            var service = new PrateleiraService(context);
            var prateleira = new Prateleira { Nome = "Prateleira para Deletar" };
            context.Prateleiras.Add(prateleira);
            await context.SaveChangesAsync();

            // Act
            await service.DeleteAsync(prateleira.Id);

            // Assert
            var deletedPrateleira = await context.Prateleiras.FindAsync(prateleira.Id);
            Assert.Null(deletedPrateleira);
        }

        [Fact]
        public async Task UpdateAsync_ExistingPrateleira_UpdatesSuccessfully()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new AppDbContext(options);
            var service = new PrateleiraService(context);
            var prateleira = new Prateleira 
            { 
                Nome = "Nome Original",
                Localizacao = "Local Original"
            };
            context.Prateleiras.Add(prateleira);
            await context.SaveChangesAsync();

            // Act
            prateleira.Nome = "Nome Atualizado";
            await service.UpdateAsync(prateleira);

            // Assert
            var updatedPrateleira = await context.Prateleiras.FindAsync(prateleira.Id);
            Assert.NotNull(updatedPrateleira);
            Assert.Equal("Nome Atualizado", updatedPrateleira.Nome);
        }
    }
}