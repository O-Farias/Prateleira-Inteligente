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
        public DbSet<Alerta> Alertas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração Produto
            modelBuilder.Entity<Produto>(entity =>
            {
                entity.HasOne(p => p.Prateleira)
                    .WithMany(p => p.Produtos)
                    .HasForeignKey(p => p.PrateleiraId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(p => p.Nome)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(p => p.Descricao)
                    .HasMaxLength(500);

                entity.Property(p => p.CodigoBarras)
                    .HasMaxLength(50);

                entity.Property(p => p.Preco)
                    .HasPrecision(10, 2);
            });

            // Configuração Categoria-Produto (many-to-many)
            modelBuilder.Entity<Produto>()
                .HasMany(p => p.Categorias)
                .WithMany(c => c.Produtos)
                .UsingEntity("ProdutoCategorias");

            // Configuração Categoria
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.Property(c => c.Nome)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(c => c.Descricao)
                    .HasMaxLength(200);
            });

            // Configuração Movimentação
            modelBuilder.Entity<Movimentacao>(entity =>
            {
                entity.HasOne(m => m.Produto)
                    .WithMany()
                    .HasForeignKey(m => m.ProdutoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(m => m.Observacao)
                    .HasMaxLength(500);
            });

            // Configuração Prateleira
            modelBuilder.Entity<Prateleira>(entity =>
            {
                entity.Property(p => p.Nome)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(p => p.Localizacao)
                    .HasMaxLength(200);
            });

            // Configuração Alerta
            modelBuilder.Entity<Alerta>(entity =>
            {
                entity.HasOne(a => a.Produto)
                .WithMany()
                .HasForeignKey(a => a.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.Property(a => a.Mensagem)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(a => a.DataCriacao)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(a => a.DataResolucao)
                    .IsRequired(false);
            });
        }
    }
}