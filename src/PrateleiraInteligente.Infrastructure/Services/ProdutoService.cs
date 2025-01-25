using Microsoft.EntityFrameworkCore;
using PrateleiraInteligente.Domain.Entities;
using PrateleiraInteligente.Domain.Interfaces;
using PrateleiraInteligente.Infrastructure.Persistence;

namespace PrateleiraInteligente.Infrastructure.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly AppDbContext _context;

        public ProdutoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Produto>> GetAllAsync()
        {
            return await _context.Produtos
                .Include(p => p.Prateleira)
                .Include(p => p.Categorias)
                .ToListAsync();
        }

        public async Task<Produto> GetByIdAsync(int id)
        {
            var produto = await _context.Produtos
                .Include(p => p.Prateleira)
                .Include(p => p.Categorias)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (produto == null)
                throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");

            return produto;
        }

        public async Task<Produto> CreateAsync(Produto produto)
        {
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();
            return produto;
        }

        public async Task UpdateAsync(Produto produto)
        {
            _context.Entry(produto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Produto>> GetProdutosBaixoEstoqueAsync(int quantidadeMinima = 5)
        {
            return await _context.Produtos
                .Include(p => p.Prateleira)
                .Include(p => p.Categorias)
                .Where(p => p.QuantidadeEstoque <= quantidadeMinima)
                .ToListAsync();
        }

        public async Task<IEnumerable<Produto>> GetProdutosProximosVencimentoAsync(int diasAviso = 7)
        {
            var dataLimite = DateTime.Now.AddDays(diasAviso);
            return await _context.Produtos
                .Include(p => p.Prateleira)
                .Include(p => p.Categorias)
                .Where(p => p.DataValidade.HasValue && p.DataValidade <= dataLimite)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Produtos.AnyAsync(p => p.Id == id);
        }
    }
}