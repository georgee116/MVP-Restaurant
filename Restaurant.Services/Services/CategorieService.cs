// Restaurant.Services/Services/CategorieService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.DataAccess.Interfaces;
using Restaurant.DataAccess.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Services.Interfaces;

namespace Restaurant.Services.Services
{
    public class CategorieService : ICategorieService
    {
        private readonly ICategorieRepository _repo;

        public CategorieService()
        {
            _repo = new CategorieRepository();
        }

        public async Task<IEnumerable<Categorie>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Categorie?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<Categorie> CreateAsync(string nume)
        {
            var categorie = new Categorie { Nume = nume };
            await _repo.AddAsync(categorie);
            return categorie;
        }

        public async Task<Categorie> UpdateAsync(Categorie categorie)
        {
            await _repo.UpdateAsync(categorie);
            return categorie;
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.RemoveAsync(id);
        }
    }
}