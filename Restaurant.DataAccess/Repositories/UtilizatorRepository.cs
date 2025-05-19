// Restaurant.DataAccess/Repositories/UtilizatorRepository.cs
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data.Context;
using Restaurant.DataAccess.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;

namespace Restaurant.DataAccess.Repositories
{
    public class UtilizatorRepository : IUtilizatorRepository
    {
        public async Task<Utilizator?> GetByEmailAndRoleAsync(string email, UserRole role)
        {
            using var ctx = new ApplicationDbContext();
            return await ctx.Utilizatori
                            .FirstOrDefaultAsync(u => u.Email == email && u.Role == role);
        }

        public async Task AddAsync(Utilizator user)
        {
            using var ctx = new ApplicationDbContext();
            ctx.Utilizatori.Add(user);
            await ctx.SaveChangesAsync();
        }
    }
}
