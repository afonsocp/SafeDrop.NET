using Microsoft.EntityFrameworkCore;
using EmergencyManagementAPI.Models;

namespace EmergencyManagementAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Ocorrencia> Ocorrencias { get; set; }
        public DbSet<TipoOcorrencia> TiposOcorrencia { get; set; }
        public DbSet<Abrigo> Abrigos { get; set; }
        public DbSet<CheckinAbrigo> CheckinsAbrigos { get; set; }
        public DbSet<Alerta> Alertas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações de relacionamentos
            modelBuilder.Entity<Ocorrencia>()
                .HasOne(o => o.TipoOcorrencia)
                .WithMany(t => t.Ocorrencias)
                .HasForeignKey(o => o.IdTipo);

            modelBuilder.Entity<CheckinAbrigo>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.CheckinsAbrigos)
                .HasForeignKey(c => c.IdUsuario);

            modelBuilder.Entity<CheckinAbrigo>()
                .HasOne(c => c.Abrigo)
                .WithMany(a => a.CheckinsAbrigos)
                .HasForeignKey(c => c.IdAbrigo);
        }
    }
}