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
    public class AlertaServiceTests
    {
        private DbContextOptions<AppDbContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Theory]
        [InlineData("Teste Alerta", TipoAlerta.EstoqueBaixo)]
        [InlineData("Vencimento Próximo", TipoAlerta.ProximoVencimento)]
        public async Task CreateAlertaAsync_ValidAlerta_ReturnsAlerta(string mensagem, TipoAlerta tipo)
        {
            // Arrange
            using var context = new AppDbContext(CreateNewContextOptions());
            var service = new AlertaService(context);
            var produto = new Produto { Nome = "Produto Teste" };
            context.Produtos.Add(produto);
            await context.SaveChangesAsync();

            var alerta = new Alerta
            {
                ProdutoId = produto.Id,
                Tipo = tipo,
                Mensagem = mensagem
            };

            // Act
            var result = await service.CreateAlertaAsync(alerta);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Resolvido);
            Assert.Equal(mensagem, result.Mensagem);
            Assert.Equal(tipo, result.Tipo);
            Assert.True(result.DataCriacao > DateTime.MinValue);
        }

        [Fact]
        public async Task GetAlertasNaoResolvidosAsync_ReturnsOnlyUnresolved()
        {
            // Arrange
            using var context = new AppDbContext(CreateNewContextOptions());
            var service = new AlertaService(context);
            var produto = new Produto { Nome = "Produto Teste" };
            context.Produtos.Add(produto);
            await context.SaveChangesAsync();

            var alertas = new[]
            {
                new Alerta { ProdutoId = produto.Id, Mensagem = "Alerta 1", Resolvido = false },
                new Alerta { ProdutoId = produto.Id, Mensagem = "Alerta 2", Resolvido = true },
                new Alerta { ProdutoId = produto.Id, Mensagem = "Alerta 3", Resolvido = false }
            };

            context.Alertas.AddRange(alertas);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetAlertasNaoResolvidosAsync();

            // Assert
            var alertasNaoResolvidos = result.ToList();
            Assert.Equal(2, alertasNaoResolvidos.Count);
            Assert.All(alertasNaoResolvidos, a => Assert.False(a.Resolvido));
        }

        [Fact]
        public async Task ResolverAlertaAsync_UnresolvedAlert_ResolvesCorrectly()
        {
            // Arrange
            using var context = new AppDbContext(CreateNewContextOptions());
            var service = new AlertaService(context);
            var produto = new Produto { Nome = "Produto Teste" };
            context.Produtos.Add(produto);
            await context.SaveChangesAsync();

            var alerta = new Alerta
            {
                ProdutoId = produto.Id,
                Mensagem = "Teste",
                Resolvido = false
            };
            context.Alertas.Add(alerta);
            await context.SaveChangesAsync();

            // Act
            await service.ResolverAlertaAsync(alerta.Id);

            // Assert
            var resolvedAlerta = await context.Alertas.FindAsync(alerta.Id);
            Assert.NotNull(resolvedAlerta);
            Assert.True(resolvedAlerta.Resolvido);
            Assert.NotNull(resolvedAlerta.DataResolucao);
        }

        [Theory]
        [InlineData(3, true)]  // Estoque baixo
        [InlineData(10, false)] // Estoque normal
        public async Task VerificarEstoqueBaixoAsync_VerificaCorretamente(int quantidade, bool deveCriarAlerta)
        {
            // Arrange
            using var context = new AppDbContext(CreateNewContextOptions());
            var service = new AlertaService(context);
            var produto = new Produto
            {
                Nome = "Produto Teste",
                QuantidadeEstoque = quantidade
            };
            context.Produtos.Add(produto);
            await context.SaveChangesAsync();

            // Act
            await service.VerificarEstoqueBaixoAsync();

            // Assert
            var alertas = await context.Alertas.ToListAsync();
            if (deveCriarAlerta)
            {
                Assert.Single(alertas);
                var alerta = alertas.First();
                Assert.Equal(TipoAlerta.EstoqueBaixo, alerta.Tipo);
            }
            else
            {
                Assert.Empty(alertas);
            }
        }

        [Theory]
        [InlineData(5, true)]   // Produto próximo do vencimento
        [InlineData(15, false)] // Produto longe do vencimento
        public async Task VerificarProdutosVencimentoAsync_VerificaCorretamente(int diasParaVencimento, bool deveCriarAlerta)
        {
            // Arrange
            using var context = new AppDbContext(CreateNewContextOptions());
            var service = new AlertaService(context);
            var produto = new Produto
            {
                Nome = "Produto Teste",
                DataValidade = DateTime.Now.AddDays(diasParaVencimento)
            };
            context.Produtos.Add(produto);
            await context.SaveChangesAsync();

            // Act
            await service.VerificarProdutosVencimentoAsync();

            // Assert
            var alertas = await context.Alertas.ToListAsync();
            if (deveCriarAlerta)
            {
                Assert.Single(alertas);
                var alerta = alertas.First();
                Assert.Equal(TipoAlerta.ProximoVencimento, alerta.Tipo);
            }
            else
            {
                Assert.Empty(alertas);
            }
        }
    }
}