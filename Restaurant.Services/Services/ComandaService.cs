using Microsoft.EntityFrameworkCore;
using Restaurant.Data.Context;
using Restaurant.DataAccess.Interfaces;
using Restaurant.DataAccess.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace Restaurant.Services.Services
{
    public class ComandaService : IComandaService
    {
        private readonly IComandaRepository _repo;

        public ComandaService()
        {
            _repo = new ComandaRepository();
        }

        public Task<Comanda> PlaseazaComandaAsync(Comanda comanda) =>
            _repo.CreateAsync(comanda);

        public Task<Comanda?> GetComandaByIdAsync(int id) =>
            _repo.GetByIdAsync(id);


        public Task AdaugaItemLaComandaAsync(int comandaId, ComandaItem item) =>
            _repo.AddItemAsync(comandaId, item);

        

            // Obține toate comenzile (pentru angajat)
            public async Task<IEnumerable<Comanda>> GetAllComenziAsync()
            {
                using var ctx = new ApplicationDbContext();
                return await ctx.Comenzi
                    .Include(c => c.Utilizator)
                    .Include(c => c.ComandaItems)
                        .ThenInclude(ci => ci.Preparat)
                    .Include(c => c.ComandaItems)
                        .ThenInclude(ci => ci.Meniu)
                    .OrderByDescending(c => c.DataComenzii)
                    .ToListAsync();
            }

            // Obține comenzile unui utilizator (pentru client)
            public async Task<IEnumerable<Comanda>> GetComenziForUserAsync(int utilizatorId)
            {
                using var ctx = new ApplicationDbContext();
                return await ctx.Comenzi
                    .Include(c => c.Utilizator)
                    .Include(c => c.ComandaItems)
                        .ThenInclude(ci => ci.Preparat)
                    .Include(c => c.ComandaItems)
                        .ThenInclude(ci => ci.Meniu)
                    .Where(c => c.UtilizatorId == utilizatorId)
                    .OrderByDescending(c => c.DataComenzii)
                    .ToListAsync();
            }
        

        // Schimbă statusul unei comenzi
        public async Task SchimbaStatusAsync(int comandaId, OrderStatus status)
        {
            using var ctx = new ApplicationDbContext();
            var comanda = await ctx.Comenzi.FindAsync(comandaId);
            if (comanda != null)
            {
                comanda.Status = status;

                // Actualizează ora estimativă de livrare dacă statusul este "În livrare"
                if (status == OrderStatus.PlecatLaClient)
                {
                    comanda.OraEstimativaLivrare = DateTime.Now.AddMinutes(30); // exemplu: 30 minute pentru livrare
                }

                await ctx.SaveChangesAsync();
            }
        }
    }
  }
