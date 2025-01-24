public interface IAlertaService
{
    Task<IEnumerable<Alerta>> GetAlertasNaoResolvidosAsync();
    Task<Alerta> CreateAlertaAsync(Alerta alerta);
    Task ResolverAlertaAsync(int id);
    Task VerificarProdutosVencimentoAsync();
    Task VerificarEstoqueBaixoAsync();
}