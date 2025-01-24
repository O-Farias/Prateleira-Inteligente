using Microsoft.EntityFrameworkCore;
using PrateleiraInteligente.Domain.Entities;

namespace PrateleiraInteligente.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Prateleira> Prateleiras { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Movimentacao> Movimentacoes { get; set; }
        public DbSet<Alimento> Alimentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração Produto
            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Prateleira)
                .WithMany(p => p.Produtos)
                .HasForeignKey(p => p.PrateleiraId);

            // Configuração Categoria-Produto (many-to-many)
            modelBuilder.Entity<Produto>()
                .HasMany(p => p.Categorias)
                .WithMany(c => c.Produtos);

            // Configuração Movimentação
            modelBuilder.Entity<Movimentacao>()
                .HasOne(m => m.Produto)
                .WithMany()
                .HasForeignKey(m => m.ProdutoId);

            // Configurações adicionais para Prateleira
            modelBuilder.Entity<Prateleira>()
                .Property(p => p.Nome)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Prateleira>()
                .Property(p => p.Localizacao)
                .HasMaxLength(200);
        }
    }
}