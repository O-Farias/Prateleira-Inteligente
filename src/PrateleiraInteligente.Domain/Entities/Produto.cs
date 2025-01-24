using System.ComponentModel.DataAnnotations;

namespace PrateleiraInteligente.Domain.Entities
{
    public class Produto
    {
        public Produto()
        {
            Categorias = new List<Categoria>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do produto é obrigatório")]
        [StringLength(100)]
        public string Nome { get; set; }

        [StringLength(500)]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O código de barras é obrigatório")]
        [StringLength(13)]
        public string CodigoBarras { get; set; }

        [Required(ErrorMessage = "A data de validade é obrigatória")]
        public DateTime DataValidade { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Preco { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int QuantidadeEstoque { get; set; }

        public int PrateleiraId { get; set; }
        public virtual Prateleira Prateleira { get; set; }

        public virtual ICollection<Categoria> Categorias { get; set; }
    }
}