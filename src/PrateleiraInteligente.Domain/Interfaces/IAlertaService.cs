using System.Collections.Generic;
using System.Threading.Tasks;
using PrateleiraInteligente.Domain.Entities;

namespace PrateleiraInteligente.Domain.Interfaces
{
    public interface IAlertaService
    {
        Task<IEnumerable<Alerta>> GetAlertasNaoResolvidosAsync();
        Task<Alerta> GetByIdAsync(int id);
        Task<Alerta> CreateAlertaAsync(Alerta alerta);
        Task ResolverAlertaAsync(int id);
        Task VerificarProdutosVencimentoAsync();
        Task VerificarEstoqueBaixoAsync();
    }
}