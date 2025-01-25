using Microsoft.EntityFrameworkCore;
using PrateleiraInteligente.Domain.Entities;
using PrateleiraInteligente.Domain.Interfaces;
using PrateleiraInteligente.Infrastructure.Persistence;

namespace PrateleiraInteligente.Infrastructure.Services
{
    public class AlertaService : IAlertaService
    {
        private readonly AppDbContext _context;
        private readonly int _diasParaVencimento = 7;
        private readonly int _estoqueMinimo = 5;

        public AlertaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Alerta>> GetAlertasNaoResolvidosAsync()
        {
            return await _context.Alertas
                .Include(a => a.Produto)
                .Where(a => !a.Resolvido)
                .OrderByDescending(a => a.DataCriacao)
                .ToListAsync();
        }

        public async Task<IEnumerable<Alerta>> GetAllAsync()
        {
            return await _context.Alertas
                .Include(a => a.Produto)
                .ToListAsync();
        }

        public async Task<Alerta> GetByIdAsync(int id)
        {
            var alerta = await _context.Alertas
                .Include(a => a.Produto)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (alerta == null)
                throw new KeyNotFoundException($"Alerta com ID {id} não encontrado.");

            return alerta;
        }

        public async Task<Alerta> CreateAlertaAsync(Alerta alerta)
        {
            alerta.DataCriacao = DateTime.Now;
            alerta.Resolvido = false;
            alerta.DataResolucao = null;

            _context.Alertas.Add(alerta);
            await _context.SaveChangesAsync();

            return alerta;
        }

        public async Task UpdateAsync(Alerta alerta)
        {
            var alertaExistente = await _context.Alertas.FindAsync(alerta.Id);
            if (alertaExistente == null)
                throw new KeyNotFoundException($"Alerta com ID {alerta.Id} não encontrado.");

            _context.Entry(alertaExistente).CurrentValues.SetValues(alerta);
            await _context.SaveChangesAsync();
        }

        public async Task ResolverAlertaAsync(int id)
        {
            var alerta = await _context.Alertas.FindAsync(id);
            if (alerta == null)
                throw new KeyNotFoundException($"Alerta com ID {id} não encontrado.");

            alerta.Resolvido = true;
            alerta.DataResolucao = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task VerificarProdutosVencimentoAsync()
        {
            var dataLimite = DateTime.Now.AddDays(_diasParaVencimento);
            var produtosProximosVencimento = await _context.Produtos
                .Where(p => p.DataValidade.HasValue && p.DataValidade <= dataLimite && p.DataValidade > DateTime.Now)
                .ToListAsync();

            foreach (var produto in produtosProximosVencimento)
            {
                var alertaExistente = await _context.Alertas
                    .AnyAsync(a => a.ProdutoId == produto.Id &&
                                  a.Tipo == TipoAlerta.ProximoVencimento &&
                                  !a.Resolvido);

                if (!alertaExistente && produto.DataValidade.HasValue)
                {
                    var diasRestantes = (produto.DataValidade.Value - DateTime.Now).Days;
                    await CreateAlertaAsync(new Alerta
                    {
                        ProdutoId = produto.Id,
                        Tipo = TipoAlerta.ProximoVencimento,
                        Mensagem = $"O produto {produto.Nome} irá vencer em {diasRestantes} dias"
                    });
                }
            }
        }

        public async Task VerificarEstoqueBaixoAsync()
        {
            var produtosBaixoEstoque = await _context.Produtos
                .Where(p => p.QuantidadeEstoque <= _estoqueMinimo)
                .ToListAsync();

            foreach (var produto in produtosBaixoEstoque)
            {
                var alertaExistente = await _context.Alertas
                    .AnyAsync(a => a.ProdutoId == produto.Id &&
                                  a.Tipo == TipoAlerta.EstoqueBaixo &&
                                  !a.Resolvido);

                if (!alertaExistente)
                {
                    await CreateAlertaAsync(new Alerta
                    {
                        ProdutoId = produto.Id,
                        Tipo = TipoAlerta.EstoqueBaixo,
                        Mensagem = $"O produto {produto.Nome} está com estoque baixo. Quantidade atual: {produto.QuantidadeEstoque}"
                    });
                }
            }
        }
    }
}