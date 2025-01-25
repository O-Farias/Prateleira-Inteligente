using System.Collections.Generic;
using System.Threading.Tasks;
using PrateleiraInteligente.Domain.Entities;

namespace PrateleiraInteligente.Domain.Interfaces
{
    public interface IPrateleiraService
    {
        Task<IEnumerable<Prateleira>> GetAllAsync();
        Task<Prateleira?> GetByIdAsync(int id);
        Task<Prateleira> CreateAsync(Prateleira prateleira);
        Task UpdateAsync(Prateleira prateleira);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> TemEspacoDisponivel(int prateleiraId);
    }
}