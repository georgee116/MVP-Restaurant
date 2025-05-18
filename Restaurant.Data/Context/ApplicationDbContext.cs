using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Restaurant.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        // tabele existente
        public DbSet<Categorie> Categorii { get; set; }
        public DbSet<Preparat> Preparate { get; set; }
        public DbSet<ImaginePreparat> ImaginiPreparat { get; set; }

        // tabele noi
        public DbSet<Alergen> Alergeni { get; set; }
        public DbSet<PreparatAlergen> PreparatAlergeni { get; set; }

        public DbSet<Meniu> Meniuri { get; set; }
        public DbSet<PreparatMeniu> PreparatMeniuri { get; set; }

        public DbSet<Utilizator> Utilizatori { get; set; }
        public DbSet<Comanda> Comenzi { get; set; }
        public DbSet<ComandaItem> ComandaItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=GEO\\MSSQLSERVER01;Database=RestaurantDB;Trusted_Connection=True;TrustServerCertificate=True;"
                );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ------ Preparat <-> Alergen (M:N) ------
            modelBuilder.Entity<PreparatAlergen>()
                .HasKey(pa => new { pa.PreparatId, pa.AlergenId });

            modelBuilder.Entity<PreparatAlergen>()
                .HasOne(pa => pa.Preparat)
                .WithMany(p => p.Alergeni)
                .HasForeignKey(pa => pa.PreparatId);

            modelBuilder.Entity<PreparatAlergen>()
                .HasOne(pa => pa.Alergen)
                .WithMany(a => a.PreparatAlergeni)
                .HasForeignKey(pa => pa.AlergenId);

            // ------ Meniu <-> Preparat (M:N) ------
            modelBuilder.Entity<PreparatMeniu>()
        .HasKey(pm => new { pm.MeniuId, pm.PreparatId });

            modelBuilder.Entity<PreparatMeniu>()
                .HasOne(pm => pm.Meniu)
                .WithMany(m => m.PreparatMeniuri)
                .HasForeignKey(pm => pm.MeniuId)
                // păstrează cascade aici, dacă vrei:
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PreparatMeniu>()
                .HasOne(pm => pm.Preparat)
                .WithMany(p => p.Meniuri)
                .HasForeignKey(pm => pm.PreparatId)
                // dezactivează cascade pe partea cealaltă:
                .OnDelete(DeleteBehavior.Restrict);

            // ------ Utilizator ------
            modelBuilder.Entity<Utilizator>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // ------ Comanda & ComandaItem ------
            modelBuilder.Entity<Comanda>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<ComandaItem>()
                .HasKey(ci => ci.Id);

            modelBuilder.Entity<ComandaItem>()
                .HasOne(ci => ci.Comanda)
                .WithMany(c => c.ComandaItems)
                .HasForeignKey(ci => ci.ComandaId);

            modelBuilder.Entity<ComandaItem>()
                .HasOne(ci => ci.Preparat)
                .WithMany()
                .HasForeignKey(ci => ci.PreparatId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ComandaItem>()
                .HasOne(ci => ci.Meniu)
                .WithMany()
                .HasForeignKey(ci => ci.MeniuId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
