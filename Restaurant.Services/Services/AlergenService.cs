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
    public class AlergenService : IAlergenService
    {
        private readonly IAlergenRepository _repo;

        public AlergenService()
        {
            _repo = new AlergenRepository();
        }

        public Task<IEnumerable<Alergen>> GetAllAlergeniAsync() =>
            _repo.GetAllAsync();

        public Task<Alergen?> GetAlergenByIdAsync(int id) =>
            _repo.GetByIdAsync(id);

        public Task<Alergen> CreateAlergenAsync(string nume) =>
            _repo.AddAsync(new Alergen { Nume = nume });

        public Task<Alergen> UpdateAlergenAsync(Alergen alergen) =>
            _repo.UpdateAsync(alergen);

        public Task DeleteAlergenAsync(int id) =>
            _repo.DeleteAsync(id);
    }
}
