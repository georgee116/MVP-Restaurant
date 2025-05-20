// Restaurant.Services/Interfaces/ICategorieService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.Domain.Entities;

namespace Restaurant.Services.Interfaces
{
    public interface ICategorieService
    {
        Task<IEnumerable<Categorie>> GetAllAsync();
        Task<Categorie?> GetByIdAsync(int id);
        Task<Categorie> CreateAsync(string nume);
        Task<Categorie> UpdateAsync(Categorie categorie);
        Task DeleteAsync(int id);
    }
}