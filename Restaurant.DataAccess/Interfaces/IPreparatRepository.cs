using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.Domain.Entities;

namespace Restaurant.DataAccess.Interfaces
{
    public interface IPreparatRepository : IRepository<Preparat>
    {
        Task UpdateStocAsync(int preparatId, float cantitateNoua);
        Task<bool> VerificaStocDisponibilAsync(int preparatId, float cantitateNecesara);
    }
}
