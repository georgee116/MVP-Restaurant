using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data.Context;
using Restaurant.DataAccess.Interfaces;
using Restaurant.Domain.Entities;

namespace Restaurant.DataAccess.Repositories
{
    public class PreparatMeniuRepository : IPreparatMeniuRepository
    {
        public async Task AddToMeniuAsync(int meniuId, int preparatId, float cantitatePortie)
        {
            using var ctx = new ApplicationDbContext();
            ctx.PreparatMeniuri.Add(new PreparatMeniu
            {
                MeniuId = meniuId,
                PreparatId = preparatId,
                CantitatePortieMeniu = cantitatePortie
            });
            await ctx.SaveChangesAsync();
        }

        // În PreparatMeniuRepository.cs
        public async Task RemoveFromMeniuAsync(int meniuId, int preparatId)
        {
            using var ctx = new ApplicationDbContext();

            var link = await ctx.PreparatMeniuri
                .Where(pm => pm.MeniuId == meniuId && pm.PreparatId == preparatId)
                .FirstOrDefaultAsync();

            if (link != null)
            {
                ctx.PreparatMeniuri.Remove(link);
                await ctx.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<PreparatMeniu>> GetPreparatMeniuriAsync(int meniuId)
        {
            using var ctx = new ApplicationDbContext();
            return await ctx.PreparatMeniuri
                            .Where(pm => pm.MeniuId == meniuId)
                            .Include(pm => pm.Preparat)
                            .ToListAsync();
        }
    }
}
