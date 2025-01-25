using System;
using System.ComponentModel.DataAnnotations;

namespace PrateleiraInteligente.Domain.Entities
{
    public class Movimentacao
    {
        public Movimentacao()
        {
            Observacao = string.Empty;
            DataMovimentacao = DateTime.Now;
        }

        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public virtual Produto Produto { get; set; } = null!;
        public TipoMovimentacao Tipo { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public int Quantidade { get; set; }

        public DateTime DataMovimentacao { get; set; }

        [StringLength(500)]
        public string Observacao { get; set; }
    }

    public enum TipoMovimentacao
    {
        Entrada,
        Saida
    }
}