using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.Domain.Entities;

namespace Restaurant.DataAccess.Interfaces
{
    public interface IPreparatAlergenRepository
    {
        Task AddLinkAsync(int preparatId, int alergenId);
        Task RemoveLinkAsync(int preparatId, int alergenId);
        Task<IEnumerable<Alergen>> GetAlergeniForPreparatAsync(int preparatId);
    }
}
