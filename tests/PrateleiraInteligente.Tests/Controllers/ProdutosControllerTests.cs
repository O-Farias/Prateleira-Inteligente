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
    public class ProdutosControllerTests
    {
        private readonly Mock<IProdutoService> _serviceMock;
        private readonly ProdutosController _controller;

        public ProdutosControllerTests()
        {
            _serviceMock = new Mock<IProdutoService>();
            _controller = new ProdutosController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetProdutos_ReturnOkResult()
        {
            // Arrange
            var produtos = new List<Produto>
            {
                new() { Id = 1, Nome = "Produto 1" },
                new() { Id = 2, Nome = "Produto 2" }
            };
            _serviceMock.Setup(s => s.GetAllAsync())
                .ReturnsAsync(produtos);

            // Act
            var result = await _controller.GetProdutos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<Produto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetProduto_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var produto = new Produto { Id = 1, Nome = "Teste" };
            _serviceMock.Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(produto);

            // Act
            var result = await _controller.GetProduto(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<Produto>(okResult.Value);
            Assert.Equal("Teste", returnValue.Nome);
        }

        [Fact]
        public async Task GetProduto_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByIdAsync(999))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.GetProduto(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateProduto_ValidProduto_ReturnsCreatedAtAction()
        {
            // Arrange
            var produto = new Produto { Nome = "Novo Produto" };
            _serviceMock.Setup(s => s.CreateAsync(It.IsAny<Produto>()))
                .ReturnsAsync((Produto p) => { p.Id = 1; return p; });

            // Act
            var result = await _controller.CreateProduto(produto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<Produto>(createdAtActionResult.Value);
            Assert.Equal("Novo Produto", returnValue.Nome);
        }

        [Fact]
        public async Task UpdateProduto_ValidUpdate_ReturnsNoContent()
        {
            // Arrange
            var produto = new Produto { Id = 1, Nome = "Atualizado" };
            _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<Produto>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateProduto(1, produto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteProduto_ExistingId_ReturnsNoContent()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteAsync(1))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteProduto(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetProdutosProximosVencimento_ReturnsOkResult()
        {
            // Arrange
            var produtos = new List<Produto>
            {
                new() { Id = 1, Nome = "Produto 1", DataValidade = DateTime.Now.AddDays(5) }
            };
            _serviceMock.Setup(s => s.GetProdutosProximosVencimentoAsync(7))
                .ReturnsAsync(produtos);

            // Act
            var result = await _controller.GetProdutosProximosVencimento(7);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<Produto>>(okResult.Value);
            Assert.Single(returnValue);
        }
    }
}