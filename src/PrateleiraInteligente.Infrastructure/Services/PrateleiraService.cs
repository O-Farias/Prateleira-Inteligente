using Microsoft.EntityFrameworkCore;
using PrateleiraInteligente.Domain.Entities;
using PrateleiraInteligente.Domain.Interfaces;
using PrateleiraInteligente.Infrastructure.Persistence;

namespace PrateleiraInteligente.Infrastructure.Services
{
    public class PrateleiraService : IPrateleiraService
    {
        private readonly AppDbContext _context;

        public PrateleiraService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Prateleira>> GetAllAsync()
        {
            return await _context.Prateleiras
                .Include(p => p.Produtos)
                .ToListAsync();
        }

        public async Task<Prateleira?> GetByIdAsync(int id)
        {
            return await _context.Prateleiras
                .Include(p => p.Produtos)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Prateleira> CreateAsync(Prateleira prateleira)
        {
            _context.Prateleiras.Add(prateleira);
            await _context.SaveChangesAsync();
            return prateleira;
        }

        public async Task UpdateAsync(Prateleira prateleira)
        {
            var existingPrateleira = await _context.Prateleiras.FindAsync(prateleira.Id);
            if (existingPrateleira == null)
                throw new KeyNotFoundException($"Prateleira com ID {prateleira.Id} não encontrada.");

            _context.Entry(existingPrateleira).CurrentValues.SetValues(prateleira);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var prateleira = await _context.Prateleiras.FindAsync(id);
            if (prateleira == null)
                throw new KeyNotFoundException($"Prateleira com ID {id} não encontrada.");

            _context.Prateleiras.Remove(prateleira);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Prateleiras.AnyAsync(p => p.Id == id);
        }

        public async Task<bool> TemEspacoDisponivel(int prateleiraId)
        {
            var prateleira = await _context.Prateleiras
                .Include(p => p.Produtos)
                .FirstOrDefaultAsync(p => p.Id == prateleiraId);

            if (prateleira == null)
                throw new KeyNotFoundException($"Prateleira com ID {prateleiraId} não encontrada.");

            return prateleira.Produtos.Count < prateleira.CapacidadeMaxima;
        }
    }
}