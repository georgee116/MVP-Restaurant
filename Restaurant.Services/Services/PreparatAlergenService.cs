using Restaurant.DataAccess.Interfaces;
using Restaurant.DataAccess.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace Restaurant.Services.Services
{
    public class PreparatAlergenService : IPreparatAlergenService
    {
        private readonly IPreparatAlergenRepository _repo;

        public PreparatAlergenService()
        {
            _repo = new PreparatAlergenRepository();
        }

        public Task AddAlergenAsync(int preparatId, int alergenId) =>
            _repo.AddLinkAsync(preparatId, alergenId);

        public Task RemoveAlergenAsync(int preparatId, int alergenId) =>
            _repo.RemoveLinkAsync(preparatId, alergenId);

        public Task<IEnumerable<Alergen>> GetAlergeniForPreparatAsync(int preparatId) =>
            _repo.GetAlergeniForPreparatAsync(preparatId);
    }
}
