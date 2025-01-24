namespace PrateleiraInteligente.Domain.Entities
{
    public class Prateleira
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Localizacao { get; set; }
        public int CapacidadeMaxima { get; set; }
        public List<Produto> Produtos { get; set; }
    }
}