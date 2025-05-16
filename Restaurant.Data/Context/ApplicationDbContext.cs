// File: Restaurant.Data/Context/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;

namespace Restaurant.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Categorie> Categorii { get; set; }
        public DbSet<Preparat> Preparate { get; set; }
        public DbSet<ImaginePreparat> ImaginiPreparat { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Înlocuieşte cu fiecare componentă a instanţei tale de SQL Server:
                //   Server=<SERVER_NAME>\<INSTANCE_NAME>;
                //   Database=<DB_NAME>;
                //   Trusted_Connection=True; 
                //   TrustServerCertificate=True; // doar dacă e nevoie
                optionsBuilder.UseSqlServer(
                    "Server=GEO\\MSSQLSERVER01;Database=RestaurantDB;Trusted_Connection=True;TrustServerCertificate=True;"
                );
            }
        }
    }
}
