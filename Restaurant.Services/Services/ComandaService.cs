using Restaurant.DataAccess.Interfaces;
using Restaurant.DataAccess.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace Restaurant.Services.Services
{
    public class ComandaService : IComandaService
    {
        private readonly IComandaRepository _repo;

        public ComandaService()
        {
            _repo = new ComandaRepository();
        }

        public Task<Comanda> PlaseazaComandaAsync(Comanda comanda) =>
            _repo.CreateAsync(comanda);

        public Task<Comanda?> GetComandaByIdAsync(int id) =>
            _repo.GetByIdAsync(id);

        public Task SchimbaStatusAsync(int comandaId, OrderStatus nouStatus) =>
            _repo.UpdateStatusAsync(comandaId, nouStatus);

        public Task AdaugaItemLaComandaAsync(int comandaId, ComandaItem item) =>
            _repo.AddItemAsync(comandaId, item);
    }
}
