using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PrateleiraInteligente.Domain.Entities
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [StringLength(500)]
        public string Descricao { get; set; } = string.Empty;

        public virtual ICollection<Produto> Produtos { get; set; }

        public Categoria()
        {
            Produtos = new List<Produto>();
        }
    }
}