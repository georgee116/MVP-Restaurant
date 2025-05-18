
using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.Domain.Entities;

namespace Restaurant.DataAccess.Interfaces
{
    public interface IAlergenRepository
    {
        Task<IEnumerable<Alergen>> GetAllAsync();
        Task<Alergen?> GetByIdAsync(int id);
        Task<Alergen> AddAsync(Alergen alergen);
        Task<Alergen> UpdateAsync(Alergen alergen);
        Task DeleteAsync(int id);
    }
}
