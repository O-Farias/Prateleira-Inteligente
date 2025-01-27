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
    public class CategoriaServiceTests
    {
        private DbContextOptions<AppDbContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task CreateAsync_ValidCategoria_ReturnsCategoria()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new AppDbContext(options);
            var service = new CategoriaService(context);
            var categoria = new Categoria 
            { 
                Nome = "Categoria Teste",
                Descricao = "Descrição teste"
            };

            // Act
            var result = await service.CreateAsync(categoria);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Categoria Teste", result.Nome);
            Assert.Equal("Descrição teste", result.Descricao);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingCategoria_ReturnsCategoria()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new AppDbContext(options);
            var service = new CategoriaService(context);
            var categoria = new Categoria 
            { 
                Nome = "Categoria Teste" 
            };
            context.Categorias.Add(categoria);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetByIdAsync(categoria.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoria.Nome, result.Nome);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingCategoria_ReturnsNull()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new AppDbContext(options);
            var service = new CategoriaService(context);

            // Act
            var result = await service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_WithCategories_ReturnsAllCategories()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new AppDbContext(options);
            var service = new CategoriaService(context);
            var categorias = new[]
            {
                new Categoria { Nome = "Categoria 1" },
                new Categoria { Nome = "Categoria 2" },
                new Categoria { Nome = "Categoria 3" }
            };
            context.Categorias.AddRange(categorias);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetAllAsync();

            // Assert
            var categoriasList = result.ToList();
            Assert.Equal(3, categoriasList.Count);
            Assert.Contains(categoriasList, c => c.Nome == "Categoria 1");
            Assert.Contains(categoriasList, c => c.Nome == "Categoria 2");
            Assert.Contains(categoriasList, c => c.Nome == "Categoria 3");
        }

        [Fact]
        public async Task DeleteAsync_ExistingCategoria_RemovesFromDatabase()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new AppDbContext(options);
            var service = new CategoriaService(context);
            var categoria = new Categoria { Nome = "Categoria para Deletar" };
            context.Categorias.Add(categoria);
            await context.SaveChangesAsync();

            // Act
            await service.DeleteAsync(categoria.Id);

            // Assert
            var deletedCategoria = await context.Categorias.FindAsync(categoria.Id);
            Assert.Null(deletedCategoria);
        }

        [Fact]
        public async Task ExistsAsync_ExistingCategoria_ReturnsTrue()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new AppDbContext(options);
            var service = new CategoriaService(context);
            var categoria = new Categoria { Nome = "Categoria Teste" };
            context.Categorias.Add(categoria);
            await context.SaveChangesAsync();

            // Act
            var exists = await service.ExistsAsync(categoria.Id);

            // Assert
            Assert.True(exists);
        }
    }
}