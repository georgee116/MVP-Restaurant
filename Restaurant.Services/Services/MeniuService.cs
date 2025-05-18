// Restaurant.Services/Services/MeniuService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.DataAccess.Interfaces;
using Restaurant.DataAccess.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Services.Services
{
    public class MeniuService
    {
        private readonly IMeniuRepository _repo = new MeniuRepository();

        public Task<IEnumerable<Meniu>> GetAllAsync()
            => _repo.GetAllAsync();

        public Task<Meniu?> GetByIdAsync(int id)
            => _repo.GetByIdAsync(id);

        public Task AddAsync(Meniu meniu)
            => _repo.AddAsync(meniu);

        public Task UpdateAsync(Meniu meniu)
            => _repo.UpdateAsync(meniu);

        public Task DeleteAsync(int id)
            => _repo.RemoveAsync(id);
    }
}
