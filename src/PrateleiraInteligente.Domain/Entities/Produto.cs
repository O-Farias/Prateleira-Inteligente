using System.ComponentModel.DataAnnotations;

namespace PrateleiraInteligente.Domain.Entities
{
    public class Produto
    {
        public Produto()
        {
            Nome = string.Empty;
            Descricao = string.Empty;
            CodigoBarras = string.Empty;
            Categorias = new List<Categoria>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do produto é obrigatório")]
        [StringLength(100)]
        public string Nome { get; set; }

        [StringLength(500)]
        public string Descricao { get; set; }

        [StringLength(50)]
        public string CodigoBarras { get; set; }

        public decimal Preco { get; set; }
        public int QuantidadeEstoque { get; set; }
        public int QuantidadeMinima { get; set; }
        public DateTime? DataValidade { get; set; }

        public int? PrateleiraId { get; set; }
        public virtual Prateleira Prateleira { get; set; } = null!;
        public virtual ICollection<Categoria> Categorias { get; set; }
    }
}