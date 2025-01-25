using System.ComponentModel.DataAnnotations;

namespace PrateleiraInteligente.Domain.Entities
{
    public class Prateleira
    {
        public Prateleira()
        {
            Nome = string.Empty;
            Localizacao = string.Empty;
            Produtos = new List<Produto>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da prateleira é obrigatório")]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A localização da prateleira é obrigatória")]
        [StringLength(200)]
        public string Localizacao { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "A capacidade máxima deve ser maior que zero")]
        public int CapacidadeMaxima { get; set; }

        public List<Produto> Produtos { get; set; }
    }
}