using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.DataAccess.Interfaces;
using Restaurant.DataAccess.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Services.Interfaces;

namespace Restaurant.Services.Services
{
    public class PreparatMeniuService : IPreparatMeniuService
    {
        private readonly IPreparatMeniuRepository _repo;

        public PreparatMeniuService()
        {
            _repo = new PreparatMeniuRepository();
        }

        public Task AddToMeniuAsync(int meniuId, int preparatId, float cantitatePortie) =>
            _repo.AddToMeniuAsync(meniuId, preparatId, cantitatePortie);

        public Task RemoveFromMeniuAsync(int meniuId, int preparatId) =>
            _repo.RemoveFromMeniuAsync(meniuId, preparatId);

        public Task<IEnumerable<PreparatMeniu>> GetPreparatMeniuriAsync(int meniuId) =>
            _repo.GetPreparatMeniuriAsync(meniuId);
    }
}
