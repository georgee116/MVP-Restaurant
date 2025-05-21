using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace Restaurant.Services.Interfaces
{
    public interface IComandaService
    {
        Task<Comanda> PlaseazaComandaAsync(Comanda comanda);
        Task<Comanda?> GetComandaByIdAsync(int id);
        Task SchimbaStatusAsync(int comandaId, OrderStatus nouStatus);
        Task AdaugaItemLaComandaAsync(int comandaId, ComandaItem item);
        Task<IEnumerable<Comanda>> GetAllComenziAsync();
        Task<IEnumerable<Comanda>> GetComenziForUserAsync(int utilizatorId);
    }
}
