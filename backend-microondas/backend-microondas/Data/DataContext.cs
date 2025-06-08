using backend_microondas.Configs;
using backend_microondas.Models;
using System.Data.Entity;

namespace backend_microondas.Data
{
    public class DataContext : DbContext
    {
        public DataContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<DataContext>());
        }

        public DbSet<ProgramaAquecimento> ProgramasAquecimento { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configuração para Usuario
            modelBuilder.Entity<Usuario>()
                .HasIndex(e => e.Nome)
                .IsUnique();

            // Configuração para ProgramaAquecimento
            modelBuilder.Entity<ProgramaAquecimento>()
                .HasIndex(e => e.Nome)
                .IsUnique();

            modelBuilder.Entity<ProgramaAquecimento>()
                .HasIndex(e => e.CaractereAquecimento)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}