// Restaurant.DataAccess/Interfaces/IImaginePreparatRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.Domain.Entities;

namespace Restaurant.DataAccess.Interfaces
{
    public interface IImaginePreparatRepository
    {
        Task<IEnumerable<ImaginePreparat>> GetByPreparatIdAsync(int preparatId);
        Task<ImaginePreparat> AddAsync(ImaginePreparat imagine);
        Task DeleteAsync(int id);
    }
}