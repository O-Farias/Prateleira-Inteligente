using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using PrateleiraInteligente.API.Controllers;
using PrateleiraInteligente.Domain.Entities;
using PrateleiraInteligente.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrateleiraInteligente.Tests.Controllers
{
    public class CategoriasControllerTests
    {
        private readonly Mock<ICategoriaService> _serviceMock;
        private readonly CategoriasController _controller;

        public CategoriasControllerTests()
        {
            _serviceMock = new Mock<ICategoriaService>();
            _controller = new CategoriasController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetCategorias_ReturnOkResultWithCategorias()
        {
            // Arrange
            var categorias = new List<Categoria>
            {
                new() { Id = 1, Nome = "Categoria 1" },
                new() { Id = 2, Nome = "Categoria 2" }
            };
            _serviceMock.Setup(s => s.GetAllAsync())
                .ReturnsAsync(categorias);

            // Act
            var result = await _controller.GetCategorias();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<Categoria>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetCategoria_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var categoria = new Categoria 
            { 
                Id = 1, 
                Nome = "Teste",
                Descricao = "Descrição teste" 
            };
            _serviceMock.Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(categoria);

            // Act
            var result = await _controller.GetCategoria(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<Categoria>(okResult.Value);
            Assert.Equal("Teste", returnValue.Nome);
        }

        [Fact]
        public async Task GetCategoria_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByIdAsync(999))
                .ReturnsAsync((Categoria)null);

            // Act
            var result = await _controller.GetCategoria(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateCategoria_ValidCategoria_ReturnsCreatedAtAction()
        {
            // Arrange
            var categoria = new Categoria 
            { 
                Nome = "Nova Categoria",
                Descricao = "Nova Descrição" 
            };
            _serviceMock.Setup(s => s.CreateAsync(It.IsAny<Categoria>()))
                .ReturnsAsync((Categoria c) => { c.Id = 1; return c; });

            // Act
            var result = await _controller.CreateCategoria(categoria);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<Categoria>(createdAtActionResult.Value);
            Assert.Equal("Nova Categoria", returnValue.Nome);
        }

        [Fact]
        public async Task CreateCategoria_InvalidCategoria_ReturnsBadRequest()
        {
            // Arrange
            Categoria? categoria = null;

            // Act
            var result = await _controller.CreateCategoria(categoria);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async Task UpdateCategoria_ValidUpdate_ReturnsNoContent()
        {
            // Arrange
            var categoria = new Categoria { Id = 1, Nome = "Atualizada" };
            _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<Categoria>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateCategoria(1, categoria);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateCategoria_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var categoria = new Categoria { Id = 1, Nome = "Teste" };

            // Act
            var result = await _controller.UpdateCategoria(2, categoria);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteCategoria_ExistingId_ReturnsNoContent()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteAsync(1))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteCategoria(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCategoria_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteAsync(999))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.DeleteCategoria(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}