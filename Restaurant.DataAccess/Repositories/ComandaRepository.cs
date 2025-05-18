using Microsoft.EntityFrameworkCore;
using Restaurant.Data.Context;
using Restaurant.DataAccess.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;



namespace Restaurant.DataAccess.Repositories
{
    public class ComandaRepository : IComandaRepository
    {
        public async Task<Comanda> CreateAsync(Comanda comanda)
        {
            using var ctx = new ApplicationDbContext();
            ctx.Comenzi.Add(comanda);
            await ctx.SaveChangesAsync();
            return comanda;
        }

        public async Task<Comanda?> GetByIdAsync(int id)
        {
            using var ctx = new ApplicationDbContext();
            return await ctx.Comenzi
                            .Include(c => c.ComandaItems)
                            .ThenInclude(ci => ci.Preparat)
                            .Include(c => c.ComandaItems)
                            .ThenInclude(ci => ci.Meniu)
                            .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateStatusAsync(int comandaId, OrderStatus status)
        {
            using var ctx = new ApplicationDbContext();
            var c = await ctx.Comenzi.FindAsync(comandaId);
            if (c == null) throw new KeyNotFoundException();
            c.Status = status;
            await ctx.SaveChangesAsync();
        }

        public async Task AddItemAsync(int comandaId, ComandaItem item)
        {
            using var ctx = new ApplicationDbContext();
            item.ComandaId = comandaId;
            ctx.ComandaItems.Add(item);
            await ctx.SaveChangesAsync();
        }
    }
}


