// Restaurant.DataAccess/Interfaces/ICategorieRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.Domain.Entities;

namespace Restaurant.DataAccess.Interfaces
{
    public interface ICategorieRepository : IRepository<Categorie>
    {
        // Toate metodele necesare sunt moștenite din IRepository<Categorie>
    }
}