namespace PrateleiraInteligente.Domain.Entities
{
    public class Alerta
    {
        public Alerta()
        {
            Mensagem = string.Empty;
            DataCriacao = DateTime.Now;
            Resolvido = false;
        }

        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public virtual Produto Produto { get; set; } = null!;
        public TipoAlerta Tipo { get; set; }
        public string Mensagem { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataResolucao { get; set; }
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