namespace PrateleiraInteligente.Domain.Entities
{
    public class Alimento
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime Validade { get; set; }
        public bool Consumido { get; set; }
    }
}
