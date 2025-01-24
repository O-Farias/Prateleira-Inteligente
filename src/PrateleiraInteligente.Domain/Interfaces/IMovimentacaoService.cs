public interface IMovimentacaoService
{
    Task<Movimentacao> RegistrarEntradaAsync(int produtoId, int quantidade, string observacao);
    Task<Movimentacao> RegistrarSaidaAsync(int produtoId, int quantidade, string observacao);
    Task<IEnumerable<Movimentacao>> GetMovimentacoesPorProdutoAsync(int produtoId);
}