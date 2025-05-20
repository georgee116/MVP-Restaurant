// Restaurant.Services/Services/ImaginePreparatService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.DataAccess.Interfaces;
using Restaurant.DataAccess.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Services.Interfaces;

namespace Restaurant.Services.Services
{
    public class ImaginePreparatService : IImaginePreparatService
    {
        private readonly IImaginePreparatRepository _repo;

        public ImaginePreparatService()
        {
            _repo = new ImaginePreparatRepository();
        }

        public async Task<IEnumerable<ImaginePreparat>> GetByPreparatIdAsync(int preparatId)
        {
            return await _repo.GetByPreparatIdAsync(preparatId);
        }

        public async Task<ImaginePreparat> AddAsync(ImaginePreparat imagine)
        {
            return await _repo.AddAsync(imagine);
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}