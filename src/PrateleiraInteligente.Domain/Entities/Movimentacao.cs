namespace PrateleiraInteligente.Domain.Entities
{
    public class Movimentacao
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; }
        public TipoMovimentacao Tipo { get; set; }
        public int Quantidade { get; set; }
        public DateTime DataMovimentacao { get; set; }
        public string Observacao { get; set; }
    }

    public enum TipoMovimentacao
    {
        Entrada,
        Saida
    }
}