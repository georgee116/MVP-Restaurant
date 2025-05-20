// Restaurant.DataAccess/Repositories/ImaginePreparatRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data.Context;
using Restaurant.DataAccess.Interfaces;
using Restaurant.Domain.Entities;

namespace Restaurant.DataAccess.Repositories
{
    public class ImaginePreparatRepository : IImaginePreparatRepository
    {
        public async Task<IEnumerable<ImaginePreparat>> GetByPreparatIdAsync(int preparatId)
        {
            using var ctx = new ApplicationDbContext();
            return await ctx.ImaginiPreparat
                           .Where(i => i.PreparatId == preparatId)
                           .ToListAsync();
        }

        public async Task<ImaginePreparat> AddAsync(ImaginePreparat imagine)
        {
            using var ctx = new ApplicationDbContext();
            ctx.ImaginiPreparat.Add(imagine);
            await ctx.SaveChangesAsync();
            return imagine;
        }

        public async Task DeleteAsync(int id)
        {
            using var ctx = new ApplicationDbContext();
            var imagine = new ImaginePreparat { Id = id };
            ctx.ImaginiPreparat.Remove(imagine);
            await ctx.SaveChangesAsync();
        }
    }
}