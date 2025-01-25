using System.Collections.Generic;
using System.Threading.Tasks;
using PrateleiraInteligente.Domain.Entities;

namespace PrateleiraInteligente.Domain.Interfaces
{
    public interface IMovimentacaoService
    {
        Task<Movimentacao> RegistrarEntradaAsync(int produtoId, int quantidade, string observacao);
        Task<Movimentacao> RegistrarSaidaAsync(int produtoId, int quantidade, string observacao);
        Task<IEnumerable<Movimentacao>> GetMovimentacoesPorProdutoAsync(int produtoId);
    }
}