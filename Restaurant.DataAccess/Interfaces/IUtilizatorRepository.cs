using System.Threading.Tasks;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;

public interface IUtilizatorRepository
{
    Task<Utilizator?> GetByEmailAndRoleAsync(string email, UserRole role);
}
