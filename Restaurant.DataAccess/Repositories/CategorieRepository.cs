// Restaurant.DataAccess/Repositories/CategorieRepository.cs
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
    public class CategorieRepository : ICategorieRepository
    {
        public async Task<IEnumerable<Categorie>> GetAllAsync()
        {
            using var ctx = new ApplicationDbContext();
            return await ctx.Categorii.ToListAsync();
        }

        public async Task<IEnumerable<Categorie>> GetAsync(
            Expression<Func<Categorie, bool>> filter = null,
            Func<IQueryable<Categorie>, IOrderedQueryable<Categorie>> orderBy = null,
            string includeProperties = "")
        {
            using var ctx = new ApplicationDbContext();
            IQueryable<Categorie> query = ctx.Categorii;

            if (filter != null)
                query = query.Where(filter);

            foreach (var include in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(include);

            if (orderBy != null)
                query = orderBy(query);

            return await query.ToListAsync();
        }

        public async Task<Categorie?> GetByIdAsync(int id)
        {
            using var ctx = new ApplicationDbContext();
            return await ctx.Categorii.FindAsync(id);
        }

        public async Task<Categorie> GetFirstOrDefaultAsync(
            Expression<Func<Categorie, bool>> filter = null,
            string includeProperties = "")
        {
            using var ctx = new ApplicationDbContext();
            IQueryable<Categorie> query = ctx.Categorii;

            if (filter != null)
                query = query.Where(filter);

            foreach (var include in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(include);

            return await query.FirstOrDefaultAsync();
        }

        public async Task AddAsync(Categorie entity)
        {
            using var ctx = new ApplicationDbContext();
            ctx.Categorii.Add(entity);
            await ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(Categorie entity)
        {
            using var ctx = new ApplicationDbContext();
            ctx.Categorii.Update(entity);
            await ctx.SaveChangesAsync();
        }

        public async Task RemoveAsync(int id)
        {
            using var ctx = new ApplicationDbContext();
            var categorie = new Categorie { Id = id };
            ctx.Categorii.Remove(categorie);
            await ctx.SaveChangesAsync();
        }

        public async Task RemoveAsync(Categorie entity)
        {
            using var ctx = new ApplicationDbContext();
            ctx.Categorii.Remove(entity);
            await ctx.SaveChangesAsync();
        }

        public async Task RemoveRangeAsync(IEnumerable<Categorie> entities)
        {
            using var ctx = new ApplicationDbContext();
            ctx.Categorii.RemoveRange(entities);
            await ctx.SaveChangesAsync();
        }
    }
}