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
        private readonly IImaginePreparatRepository _imagineRepo = new ImaginePreparatRepository();

        // Metoda nou adăugată
        public async Task<IEnumerable<ImaginePreparat>> GetImaginiPreparatAsync(int preparatId)
        {
            return await _imagineRepo.GetByPreparatIdAsync(preparatId);
        }

        // Metodele existente
        public Task<IEnumerable<Preparat>> GetAllAsync()
            => _repo.GetAllAsync();

        public Task<Preparat?> GetByIdAsync(int id)
            => _repo.GetByIdAsync(id);

        public Task AddAsync(Preparat p)
            => _repo.AddAsync(p);

        public Task UpdateAsync(Preparat p)
            => _repo.UpdateAsync(p);

        public Task DeleteAsync(int id)
            => _repo.RemoveAsync(id);

        public async Task UpdateStocAsync(int preparatId, float cantitateNoua)
        {
            await _repo.UpdateStocAsync(preparatId, cantitateNoua);
        }

        public async Task<bool> VerificaStocDisponibilAsync(int preparatId, float cantitateNecesara)
        {
            return await _repo.VerificaStocDisponibilAsync(preparatId, cantitateNecesara);
        }

        public async Task ScadeStocAsync(int preparatId, float cantitate)
        {
            var preparat = await _repo.GetByIdAsync(preparatId);
            if (preparat != null)
            {
                float cantitateNoua = Math.Max(0, preparat.CantitateTotala - cantitate);
                await _repo.UpdateStocAsync(preparatId, cantitateNoua);
            }
        }
    }
}