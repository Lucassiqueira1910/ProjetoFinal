using Microsoft.EntityFrameworkCore;
using USUARIOminimalSolution.Domain.Entities;

namespace USUARIOminimalSolution.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(b =>
            {
                b.ToTable("SW_USUARIO");            // Nome da tabela no Oracle

                b.HasKey(u => u.Id_Usuario);

                b.Property(u => u.Id_Usuario)
                    .HasColumnName("ID_USUARIO")
                    .ValueGeneratedOnAdd();

                b.Property(u => u.Nome)
                    .HasColumnName("NOME")
                    .HasMaxLength(100)
                    .IsRequired();

                b.Property(u => u.Email)
                    .HasColumnName("EMAIL")
                    .HasMaxLength(150)
                    .IsRequired();

                b.Property(u => u.Senha)
                    .HasColumnName("SENHA")
                    .HasMaxLength(200)
                    .IsRequired();

                
                b.Ignore(u => u.CreatedAt);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
