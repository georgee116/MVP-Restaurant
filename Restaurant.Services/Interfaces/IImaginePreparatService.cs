// Restaurant.Services/Interfaces/IImaginePreparatService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.Domain.Entities;

namespace Restaurant.Services.Interfaces
{
    public interface IImaginePreparatService
    {
        Task<IEnumerable<ImaginePreparat>> GetByPreparatIdAsync(int preparatId);
        Task<ImaginePreparat> AddAsync(ImaginePreparat imagine);
        Task DeleteAsync(int id);
    }
}