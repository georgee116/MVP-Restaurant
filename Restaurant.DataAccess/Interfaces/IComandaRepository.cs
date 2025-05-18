using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.DataAccess.Interfaces
{
    public interface IComandaRepository
    {
        Task<Comanda> CreateAsync(Comanda comanda);
        Task<Comanda?> GetByIdAsync(int id);
        Task UpdateStatusAsync(int comandaId, OrderStatus status);
        Task AddItemAsync(int comandaId, ComandaItem item);
        // eventual: Task<IEnumerable<Comanda>> GetForUserAsync(int utilizatorId);
    }
}
