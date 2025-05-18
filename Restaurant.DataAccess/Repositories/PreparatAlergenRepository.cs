using Microsoft.EntityFrameworkCore;
using Restaurant.Data.Context;
using Restaurant.DataAccess.Interfaces;
using Restaurant.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace Restaurant.DataAccess.Repositories
{
    public class PreparatAlergenRepository : IPreparatAlergenRepository
    {
        public async Task AddLinkAsync(int preparatId, int alergenId)
        {
            using var ctx = new ApplicationDbContext();
            ctx.PreparatAlergeni.Add(new PreparatAlergen
            {
                PreparatId = preparatId,
                AlergenId = alergenId
            });
            await ctx.SaveChangesAsync();
        }

        public async Task RemoveLinkAsync(int preparatId, int alergenId)
        {
            using var ctx = new ApplicationDbContext();
            var link = new PreparatAlergen { PreparatId = preparatId, AlergenId = alergenId };
            ctx.PreparatAlergeni.Remove(link);
            await ctx.SaveChangesAsync();
        }

        public async Task<IEnumerable<Alergen>> GetAlergeniForPreparatAsync(int preparatId)
        {
            using var ctx = new ApplicationDbContext();
            return await ctx.PreparatAlergeni
                            .Where(pa => pa.PreparatId == preparatId)
                            .Include(pa => pa.Alergen)
                            .Select(pa => pa.Alergen)
                            .ToListAsync();
        }
    }
}
