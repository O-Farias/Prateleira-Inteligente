namespace PrateleiraInteligente.Domain.Entities
{
    public class Alerta
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; }
        public TipoAlerta Tipo { get; set; }
        public string Mensagem { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool Resolvido { get; set; }
    }

    public enum TipoAlerta
    {
        ProximoVencimento,
        EstoqueBaixo,
        ForaEstoque,
        Vencido
    }
}