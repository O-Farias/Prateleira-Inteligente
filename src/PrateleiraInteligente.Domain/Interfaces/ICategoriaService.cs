using System.Collections.Generic;
using System.Threading.Tasks;
using PrateleiraInteligente.Domain.Entities;

namespace PrateleiraInteligente.Domain.Interfaces
{
    public interface ICategoriaService
    {
        Task<IEnumerable<Categoria>> GetAllAsync();
        Task<Categoria?> GetByIdAsync(int id);
        Task<Categoria> CreateAsync(Categoria categoria);
        Task UpdateAsync(Categoria categoria);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}