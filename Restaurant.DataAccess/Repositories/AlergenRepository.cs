using Microsoft.EntityFrameworkCore;
using Restaurant.Data.Context;
using Restaurant.DataAccess.Interfaces;
using Restaurant.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Restaurant.DataAccess.Repositories
{
    public class AlergenRepository : IAlergenRepository
    {
        public async Task<IEnumerable<Alergen>> GetAllAsync()
        {
            using var ctx = new ApplicationDbContext();
            return await ctx.Alergeni.ToListAsync();
        }

        public async Task<Alergen?> GetByIdAsync(int id)
        {
            using var ctx = new ApplicationDbContext();
            return await ctx.Alergeni.FindAsync(id);
        }

        public async Task<Alergen> AddAsync(Alergen alergen)
        {
            using var ctx = new ApplicationDbContext();
            ctx.Alergeni.Add(alergen);
            await ctx.SaveChangesAsync();
            return alergen;
        }

        public async Task<Alergen> UpdateAsync(Alergen alergen)
        {
            using var ctx = new ApplicationDbContext();
            ctx.Alergeni.Update(alergen);
            await ctx.SaveChangesAsync();
            return alergen;
        }

        public async Task DeleteAsync(int id)
        {
            using var ctx = new ApplicationDbContext();
            var a = new Alergen { Id = id };
            ctx.Alergeni.Remove(a);
            await ctx.SaveChangesAsync();
        }
    }
}
