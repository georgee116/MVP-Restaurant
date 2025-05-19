// Restaurant.DataAccess/Interfaces/IUtilizatorRepository.cs
using System.Threading.Tasks;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;

namespace Restaurant.DataAccess.Interfaces
{
    public interface IUtilizatorRepository
    {
        Task<Utilizator?> GetByEmailAndRoleAsync(string email, UserRole role);
        Task AddAsync(Utilizator user);
    }
}
