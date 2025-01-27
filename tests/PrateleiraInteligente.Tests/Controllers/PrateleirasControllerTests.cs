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
    public class PrateleirasControllerTests
    {
        private readonly Mock<IPrateleiraService> _serviceMock;
        private readonly PrateleirasController _controller;

        public PrateleirasControllerTests()
        {
            _serviceMock = new Mock<IPrateleiraService>();
            _controller = new PrateleirasController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetPrateleiras_ReturnOkResultWithPrateleiras()
        {
            // Arrange
            var prateleiras = new List<Prateleira>
            {
                new() { Id = 1, Nome = "Prateleira 1", CapacidadeMaxima = 10 },
                new() { Id = 2, Nome = "Prateleira 2", CapacidadeMaxima = 15 }
            };
            _serviceMock.Setup(s => s.GetAllAsync())
                .ReturnsAsync(prateleiras);

            // Act
            var result = await _controller.GetPrateleiras();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<Prateleira>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetPrateleira_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var prateleira = new Prateleira 
            { 
                Id = 1, 
                Nome = "Teste",
                Localizacao = "Setor A",
                CapacidadeMaxima = 10
            };
            _serviceMock.Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(prateleira);

            // Act
            var result = await _controller.GetPrateleira(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<Prateleira>(okResult.Value);
            Assert.Equal("Teste", returnValue.Nome);
        }

        [Fact]
        public async Task GetPrateleira_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByIdAsync(999))
                .ReturnsAsync((Prateleira)null);

            // Act
            var result = await _controller.GetPrateleira(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreatePrateleira_ValidPrateleira_ReturnsCreatedAtAction()
        {
            // Arrange
            var prateleira = new Prateleira 
            { 
                Nome = "Nova Prateleira",
                Localizacao = "Setor B",
                CapacidadeMaxima = 20
            };
            _serviceMock.Setup(s => s.CreateAsync(It.IsAny<Prateleira>()))
                .ReturnsAsync((Prateleira p) => { p.Id = 1; return p; });

            // Act
            var result = await _controller.CreatePrateleira(prateleira);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<Prateleira>(createdAtActionResult.Value);
            Assert.Equal("Nova Prateleira", returnValue.Nome);
        }

        [Fact]
        public async Task UpdatePrateleira_ValidUpdate_ReturnsNoContent()
        {
            // Arrange
            var prateleira = new Prateleira { Id = 1, Nome = "Atualizada" };
            _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<Prateleira>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdatePrateleira(1, prateleira);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdatePrateleira_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var prateleira = new Prateleira { Id = 1, Nome = "Teste" };

            // Act
            var result = await _controller.UpdatePrateleira(2, prateleira);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeletePrateleira_ExistingId_ReturnsNoContent()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteAsync(1))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeletePrateleira(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task VerificarEspacoDisponivel_ExistingId_ReturnsOkResult()
        {
            // Arrange
            _serviceMock.Setup(s => s.TemEspacoDisponivel(1))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.VerificarEspacoDisponivel(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var temEspaco = Assert.IsType<bool>(okResult.Value);
            Assert.True(temEspaco);
        }

        [Fact]
        public async Task VerificarEspacoDisponivel_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _serviceMock.Setup(s => s.TemEspacoDisponivel(999))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.VerificarEspacoDisponivel(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}