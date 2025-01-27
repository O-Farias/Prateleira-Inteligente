using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using PrateleiraInteligente.API.Controllers;
using PrateleiraInteligente.Domain.Entities;
using PrateleiraInteligente.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace PrateleiraInteligente.Tests.Controllers
{
    public class AlertasControllerTests
    {
        private readonly Mock<IAlertaService> _serviceMock;
        private readonly AlertasController _controller;

        public AlertasControllerTests()
        {
            _serviceMock = new Mock<IAlertaService>();
            _controller = new AlertasController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetAlertas_ReturnOkResultWithAlertas()
        {
            // Arrange
            var alertas = new List<Alerta>
            {
                new() {
                    Id = 1,
                    Mensagem = "Alerta 1",
                    Tipo = TipoAlerta.EstoqueBaixo,
                    Resolvido = false
                },
                new() {
                    Id = 2,
                    Mensagem = "Alerta 2",
                    Tipo = TipoAlerta.ProximoVencimento,
                    Resolvido = false
                }
            };

            _serviceMock.Setup(s => s.GetAlertasNaoResolvidosAsync())
                .ReturnsAsync(alertas);

            // Act
            var actionResult = await _controller.GetAlertas();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<Alerta>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetAlerta_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var alerta = new Alerta
            {
                Id = 1,
                Mensagem = "Teste",
                Tipo = TipoAlerta.EstoqueBaixo
            };

            _serviceMock.Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(alerta);

            // Act
            var actionResult = await _controller.GetAlerta(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<Alerta>(okResult.Value);
            Assert.Equal("Teste", returnValue.Mensagem);
        }

        [Fact]
        public async Task GetAlerta_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByIdAsync(999))
                .ReturnsAsync((Alerta?)null);

            // Act
            var actionResult = await _controller.GetAlerta(999);

            // Assert
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task CreateAlerta_ValidAlerta_ReturnsCreatedAtAction()
        {
            // Arrange
            var alerta = new Alerta
            {
                ProdutoId = 1,
                Tipo = TipoAlerta.EstoqueBaixo,
                Mensagem = "Novo Alerta"
            };

            _serviceMock.Setup(s => s.CreateAlertaAsync(It.IsAny<Alerta>()))
                .ReturnsAsync((Alerta a) => { a.Id = 1; return a; });

            // Act
            var actionResult = await _controller.CreateAlerta(alerta);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.NotNull(createdAtActionResult.Value);
            var returnValue = Assert.IsType<Alerta>(createdAtActionResult.Value);
            Assert.Equal("Novo Alerta", returnValue.Mensagem);
        }

        [Fact]
        public async Task ResolverAlerta_ExistingId_ReturnsNoContent()
        {
            // Arrange
            _serviceMock.Setup(s => s.ResolverAlertaAsync(1))
                .Returns(Task.CompletedTask);

            // Act
            var actionResult = await _controller.ResolverAlerta(1);

            // Assert
            Assert.IsType<NoContentResult>(actionResult);
        }

        [Fact]
        public async Task ResolverAlerta_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _serviceMock.Setup(s => s.ResolverAlertaAsync(999))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var actionResult = await _controller.ResolverAlerta(999);

            // Assert
            Assert.IsType<NotFoundResult>(actionResult);
        }

        [Fact]
        public async Task VerificarVencimentos_ExecutaComSucesso_ReturnsOk()
        {
            // Arrange
            _serviceMock.Setup(s => s.VerificarProdutosVencimentoAsync())
                .Returns(Task.CompletedTask);

            // Act
            var actionResult = await _controller.VerificarVencimentos();

            // Assert
            Assert.IsType<OkResult>(actionResult);
        }

        [Fact]
        public async Task VerificarEstoque_ExecutaComSucesso_ReturnsOk()
        {
            // Arrange
            _serviceMock.Setup(s => s.VerificarEstoqueBaixoAsync())
                .Returns(Task.CompletedTask);

            // Act
            var actionResult = await _controller.VerificarEstoque();

            // Assert
            Assert.IsType<OkResult>(actionResult);
        }
    }
}