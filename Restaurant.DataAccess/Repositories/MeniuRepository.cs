using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data.Context;
using Restaurant.DataAccess.Interfaces;
using Restaurant.Domain.Entities;

namespace Restaurant.DataAccess.Repositories
{
    public class MeniuRepository : IMeniuRepository
    {
        public async Task<IEnumerable<Meniu>> GetAllAsync()
        {
            using var ctx = new ApplicationDbContext();
            return await ctx.Meniuri.ToListAsync();
        }

        public async Task<IEnumerable<Meniu>> GetAsync(
            Expression<Func<Meniu, bool>> filter = null,
            Func<IQueryable<Meniu>, IOrderedQueryable<Meniu>> orderBy = null,
            string includeProperties = "")
        {
            using var ctx = new ApplicationDbContext();
            IQueryable<Meniu> query = ctx.Meniuri;

            if (filter != null)
                query = query.Where(filter);

            foreach (var include in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(include);

            if (orderBy != null)
                query = orderBy(query);

            return await query.ToListAsync();
        }

        public async Task<Meniu?> GetByIdAsync(int id)
        {
            using var ctx = new ApplicationDbContext();
            return await ctx.Meniuri.FindAsync(id);
        }

        public async Task<Meniu> GetFirstOrDefaultAsync(
            Expression<Func<Meniu, bool>> filter = null,
            string includeProperties = "")
        {
            using var ctx = new ApplicationDbContext();
            IQueryable<Meniu> query = ctx.Meniuri;

            if (filter != null)
                query = query.Where(filter);

            foreach (var include in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(include);

            return await query.FirstOrDefaultAsync();
        }

        public async Task AddAsync(Meniu entity)
        {
            using var ctx = new ApplicationDbContext();
            ctx.Meniuri.Add(entity);
            await ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(Meniu entity)
        {
            using var ctx = new ApplicationDbContext();
            ctx.Meniuri.Update(entity);
            await ctx.SaveChangesAsync();
        }

        public async Task RemoveAsync(int id)
        {
            using var ctx = new ApplicationDbContext();
            ctx.Meniuri.Remove(new Meniu { Id = id });
            await ctx.SaveChangesAsync();
        }

        public async Task RemoveAsync(Meniu entity)
        {
            using var ctx = new ApplicationDbContext();
            ctx.Meniuri.Remove(entity);
            await ctx.SaveChangesAsync();
        }

        public async Task RemoveRangeAsync(IEnumerable<Meniu> entities)
        {
            using var ctx = new ApplicationDbContext();
            ctx.Meniuri.RemoveRange(entities);
            await ctx.SaveChangesAsync();
        }
    }
}
