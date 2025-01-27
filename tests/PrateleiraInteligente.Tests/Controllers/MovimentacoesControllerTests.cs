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
    public class MovimentacoesControllerTests
    {
        private readonly Mock<IMovimentacaoService> _serviceMock;
        private readonly MovimentacoesController _controller;

        public MovimentacoesControllerTests()
        {
            _serviceMock = new Mock<IMovimentacaoService>();
            _controller = new MovimentacoesController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetMovimentacoesPorProduto_ReturnsOkResult()
        {
            // Arrange
            var movimentacoes = new List<Movimentacao>
            {
                new() 
                { 
                    Id = 1,
                    ProdutoId = 1,
                    Tipo = TipoMovimentacao.Entrada,
                    Quantidade = 5
                },
                new() 
                { 
                    Id = 2,
                    ProdutoId = 1,
                    Tipo = TipoMovimentacao.Saida,
                    Quantidade = 2
                }
            };

            _serviceMock.Setup(s => s.GetMovimentacoesPorProdutoAsync(1))
                .ReturnsAsync(movimentacoes);

            // Act
            var result = await _controller.GetMovimentacoesPorProduto(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<Movimentacao>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task RegistrarEntrada_ValidMovimentacao_ReturnsCreatedAtAction()
        {
            // Arrange
            var movimentacao = new Movimentacao
            {
                Id = 1,
                ProdutoId = 1,
                Tipo = TipoMovimentacao.Entrada,
                Quantidade = 5,
                Observacao = "Teste entrada"
            };

            _serviceMock.Setup(s => s.RegistrarEntradaAsync(1, 5, "Teste entrada"))
                .ReturnsAsync(movimentacao);

            // Act
            var result = await _controller.RegistrarEntrada(1, 5, "Teste entrada");

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<Movimentacao>(createdAtActionResult.Value);
            Assert.Equal(TipoMovimentacao.Entrada, returnValue.Tipo);
            Assert.Equal(5, returnValue.Quantidade);
        }

        [Fact]
        public async Task RegistrarEntrada_ProdutoNaoEncontrado_ReturnsNotFound()
        {
            // Arrange
            _serviceMock.Setup(s => s.RegistrarEntradaAsync(999, 5, "Teste"))
                .ThrowsAsync(new KeyNotFoundException("Produto não encontrado"));

            // Act
            var result = await _controller.RegistrarEntrada(999, 5, "Teste");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Produto não encontrado", notFoundResult.Value);
        }

        [Fact]
        public async Task RegistrarSaida_ValidMovimentacao_ReturnsCreatedAtAction()
        {
            // Arrange
            var movimentacao = new Movimentacao
            {
                Id = 1,
                ProdutoId = 1,
                Tipo = TipoMovimentacao.Saida,
                Quantidade = 3,
                Observacao = "Teste saída"
            };

            _serviceMock.Setup(s => s.RegistrarSaidaAsync(1, 3, "Teste saída"))
                .ReturnsAsync(movimentacao);

            // Act
            var result = await _controller.RegistrarSaida(1, 3, "Teste saída");

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<Movimentacao>(createdAtActionResult.Value);
            Assert.Equal(TipoMovimentacao.Saida, returnValue.Tipo);
            Assert.Equal(3, returnValue.Quantidade);
        }

        [Fact]
        public async Task RegistrarSaida_EstoqueInsuficiente_ReturnsBadRequest()
        {
            // Arrange
            _serviceMock.Setup(s => s.RegistrarSaidaAsync(1, 100, "Teste"))
                .ThrowsAsync(new InvalidOperationException("Estoque insuficiente"));

            // Act
            var result = await _controller.RegistrarSaida(1, 100, "Teste");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Estoque insuficiente", badRequestResult.Value);
        }

        [Fact]
        public async Task RegistrarSaida_ProdutoNaoEncontrado_ReturnsNotFound()
        {
            // Arrange
            _serviceMock.Setup(s => s.RegistrarSaidaAsync(999, 5, "Teste"))
                .ThrowsAsync(new KeyNotFoundException("Produto não encontrado"));

            // Act
            var result = await _controller.RegistrarSaida(999, 5, "Teste");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Produto não encontrado", notFoundResult.Value);
        }
    }
}