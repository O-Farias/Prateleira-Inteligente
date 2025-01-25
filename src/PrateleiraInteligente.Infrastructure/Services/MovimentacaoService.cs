using Microsoft.EntityFrameworkCore;
using PrateleiraInteligente.Domain.Entities;
using PrateleiraInteligente.Domain.Interfaces;
using PrateleiraInteligente.Infrastructure.Persistence;

namespace PrateleiraInteligente.Infrastructure.Services
{
    public class MovimentacaoService : IMovimentacaoService
    {
        private readonly AppDbContext _context;

        public MovimentacaoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Movimentacao> RegistrarEntradaAsync(int produtoId, int quantidade, string observacao)
        {
            var produto = await _context.Produtos.FindAsync(produtoId);
            if (produto == null)
                throw new KeyNotFoundException($"Produto com ID {produtoId} não encontrado.");

            var movimentacao = new Movimentacao
            {
                ProdutoId = produtoId,
                Tipo = TipoMovimentacao.Entrada,
                Quantidade = quantidade,
                DataMovimentacao = DateTime.Now,
                Observacao = observacao
            };

            produto.QuantidadeEstoque += quantidade;

            _context.Movimentacoes.Add(movimentacao);
            await _context.SaveChangesAsync();

            return movimentacao;
        }

        public async Task<Movimentacao> RegistrarSaidaAsync(int produtoId, int quantidade, string observacao)
        {
            var produto = await _context.Produtos.FindAsync(produtoId);
            if (produto == null)
                throw new KeyNotFoundException($"Produto com ID {produtoId} não encontrado.");

            if (produto.QuantidadeEstoque < quantidade)
                throw new InvalidOperationException("Quantidade insuficiente em estoque.");

            var movimentacao = new Movimentacao
            {
                ProdutoId = produtoId,
                Tipo = TipoMovimentacao.Saida,
                Quantidade = quantidade,
                DataMovimentacao = DateTime.Now,
                Observacao = observacao
            };

            produto.QuantidadeEstoque -= quantidade;

            _context.Movimentacoes.Add(movimentacao);
            await _context.SaveChangesAsync();

            return movimentacao;
        }

        public async Task<IEnumerable<Movimentacao>> GetMovimentacoesPorProdutoAsync(int produtoId)
        {
            return await _context.Movimentacoes
                .Include(m => m.Produto)
                .Where(m => m.ProdutoId == produtoId)
                .OrderByDescending(m => m.DataMovimentacao)
                .ToListAsync();
        }
    }
}