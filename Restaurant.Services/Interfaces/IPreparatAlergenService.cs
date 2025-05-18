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
    public interface IPreparatAlergenService
    {
        Task AddAlergenAsync(int preparatId, int alergenId);
        Task RemoveAlergenAsync(int preparatId, int alergenId);
        Task<IEnumerable<Alergen>> GetAlergeniForPreparatAsync(int preparatId);
    }
}
