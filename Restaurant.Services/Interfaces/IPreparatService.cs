// Restaurant.Services/Interfaces/IPreparatService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.Domain.Entities;

namespace Restaurant.Services.Interfaces
{
    public interface IPreparatService
    {
        Task<IEnumerable<Preparat>> GetAllAsync();
        Task<Preparat?> GetByIdAsync(int id);
        Task AddAsync(Preparat preparat);
        Task UpdateAsync(Preparat preparat);
        Task DeleteAsync(int id);
        // Metoda nouă adăugată
        Task<IEnumerable<ImaginePreparat>> GetImaginiPreparatAsync(int preparatId);

        Task UpdateStocAsync(int preparatId, float cantitateNoua);
        Task<bool> VerificaStocDisponibilAsync(int preparatId, float cantitateNecesara);
        Task ScadeStocAsync(int preparatId, float cantitate);
    }
}