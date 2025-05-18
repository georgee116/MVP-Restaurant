// Restaurant.DataAccess/Repositories/PreparatRepository.cs
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
    public class PreparatRepository : IPreparatRepository
    {
        // 1. Returnează toate entitățile
        public async Task<IEnumerable<Preparat>> GetAllAsync()
        {
            using var ctx = new ApplicationDbContext();
            return await ctx.Preparate.ToListAsync();
        }

        // 2. Filtrare, sortare, include-uri dinamice
        public async Task<IEnumerable<Preparat>> GetAsync(
            Expression<Func<Preparat, bool>> filter = null,
            Func<IQueryable<Preparat>, IOrderedQueryable<Preparat>> orderBy = null,
            string includeProperties = "")
        {
            using var ctx = new ApplicationDbContext();
            IQueryable<Preparat> query = ctx.Preparate;

            if (filter != null)
                query = query.Where(filter);

            foreach (var include in includeProperties
                         .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(include);
            }

            if (orderBy != null)
                query = orderBy(query);

            return await query.ToListAsync();
        }

        // 3. Găsește după cheia primară
        public async Task<Preparat> GetByIdAsync(int id)
        {
            using var ctx = new ApplicationDbContext();
            return await ctx.Preparate.FindAsync(id);
        }

        // 4. Primul care corespunde filtrelor + include-uri
        public async Task<Preparat> GetFirstOrDefaultAsync(
            Expression<Func<Preparat, bool>> filter = null,
            string includeProperties = "")
        {
            using var ctx = new ApplicationDbContext();
            IQueryable<Preparat> query = ctx.Preparate;

            if (filter != null)
                query = query.Where(filter);

            foreach (var include in includeProperties
                         .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync();
        }

        // 5. Adaugă un nou Preparat
        public async Task AddAsync(Preparat entity)
        {
            using var ctx = new ApplicationDbContext();
            ctx.Preparate.Add(entity);
            await ctx.SaveChangesAsync();
        }

        // 6. Actualizează Preparatul
        public async Task UpdateAsync(Preparat entity)
        {
            using var ctx = new ApplicationDbContext();
            ctx.Preparate.Update(entity);
            await ctx.SaveChangesAsync();
        }

        // 7. Şterge după Id
        public async Task RemoveAsync(int id)
        {
            using var ctx = new ApplicationDbContext();
            ctx.Preparate.Remove(new Preparat { Id = id });
            await ctx.SaveChangesAsync();
        }

        // 8. Şterge după entitate
        public async Task RemoveAsync(Preparat entity)
        {
            using var ctx = new ApplicationDbContext();
            ctx.Preparate.Remove(entity);
            await ctx.SaveChangesAsync();
        }

        // 9. Şterge mai multe entități
        public async Task RemoveRangeAsync(IEnumerable<Preparat> entities)
        {
            using var ctx = new ApplicationDbContext();
            ctx.Preparate.RemoveRange(entities);
            await ctx.SaveChangesAsync();
        }

        // 10. (opțional) alias DeleteAsync
        public Task DeleteAsync(int id) => RemoveAsync(id);
    }
}
