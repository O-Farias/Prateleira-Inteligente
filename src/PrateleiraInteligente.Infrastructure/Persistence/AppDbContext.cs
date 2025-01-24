using Microsoft.EntityFrameworkCore;
using PrateleiraInteligente.Domain.Entities;

namespace PrateleiraInteligente.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Alimento> Alimentos { get; set; }
    }
}
