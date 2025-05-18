using System.Threading.Tasks;
using Restaurant.DataAccess.Interfaces;
using Restaurant.DataAccess.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;

namespace Restaurant.Services.Services
{
    public class UtilizatorService
    {
        private readonly IUtilizatorRepository _repo = new UtilizatorRepository();

        public async Task<Utilizator?> AuthenticateAsync(
            string email,
            string password,
            UserRole role)
        {
            var user = await _repo.GetByEmailAndRoleAsync(email, role);
            if (user != null && user.Parola == password)  // hash/verify in production!
                return user;
            return null;
        }
    }
}
