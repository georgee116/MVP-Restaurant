using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.Domain.Entities;

namespace Restaurant.Services.Interfaces
{
    public interface IAlergenService
    {
        Task<IEnumerable<Alergen>> GetAllAlergeniAsync();
        Task<Alergen?> GetAlergenByIdAsync(int id);
        Task<Alergen> CreateAlergenAsync(string nume);
        Task<Alergen> UpdateAlergenAsync(Alergen alergen);
        Task DeleteAlergenAsync(int id);
    }
}
