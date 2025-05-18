// Restaurant.Services/Services/PreparatService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.DataAccess.Interfaces;
using Restaurant.DataAccess.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Services.Interfaces;

namespace Restaurant.Services.Services
{
    public class PreparatService : IPreparatService
    {
        private readonly IPreparatRepository _repo = new PreparatRepository();

        public Task<IEnumerable<Preparat>> GetAllAsync()
            => _repo.GetAllAsync();

        public Task<Preparat?> GetByIdAsync(int id)
            => _repo.GetByIdAsync(id);

        // These now return Task, not Task<Preparat>
        public Task AddAsync(Preparat p)
            => _repo.AddAsync(p);

        public Task UpdateAsync(Preparat p)
            => _repo.UpdateAsync(p);

        // And call RemoveAsync (not DeleteAsync)
        public Task DeleteAsync(int id)
            => _repo.RemoveAsync(id);
    }
}
