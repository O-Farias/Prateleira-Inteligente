using PrateleiraInteligente.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrateleiraInteligente.Domain.Interfaces
{
    public interface IProdutoService
    {
        Task<IEnumerable<Produto>> GetAllAsync();
        Task<Produto> GetByIdAsync(int id);
        Task<Produto> CreateAsync(Produto produto);
        Task UpdateAsync(Produto produto);
        Task DeleteAsync(int id);
        Task<IEnumerable<Produto>> GetProdutosProximosVencimentoAsync(int diasAviso = 7);
        Task<IEnumerable<Produto>> GetProdutosBaixoEstoqueAsync(int quantidadeMinima = 5);
        Task<bool> ExistsAsync(int id);
    }
}