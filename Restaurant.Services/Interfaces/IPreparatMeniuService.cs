using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.Domain.Entities;

namespace Restaurant.Services.Interfaces
{
    public interface IPreparatMeniuService
    {
        Task AddToMeniuAsync(int meniuId, int preparatId, float cantitatePortie);
        Task RemoveFromMeniuAsync(int meniuId, int preparatId);
        Task<IEnumerable<PreparatMeniu>> GetPreparatMeniuriAsync(int meniuId);
    }
}
