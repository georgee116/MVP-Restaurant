using Microsoft.EntityFrameworkCore;
using Restaurant.Data.Context;
using Restaurant.DataAccess.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using System.Threading.Tasks;

public class UtilizatorRepository : IUtilizatorRepository
{
    public async Task<Utilizator?> GetByEmailAndRoleAsync(string email, UserRole role)
    {
        using var ctx = new ApplicationDbContext();
        return await ctx.Utilizatori
                        .FirstOrDefaultAsync(u => u.Email == email && u.Role == role);
    }
}
