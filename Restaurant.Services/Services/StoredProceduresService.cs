using Restaurant.DataAccess.Repositories;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Entities;
using System.Data;           // for DataTable
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Restaurant.Services.Services
{
    public class StoredProceduresService
    {
        private readonly StoredProceduresRepository _repo = new StoredProceduresRepository();

        public Task<IEnumerable<Alergen>> GetAllAlergeniAsync()
            => _repo.GetAllAlergeniAsync();

        public Task AddItemToOrderAsync(
            int comandaId,
            int preparatId,
            int? meniuId,
            int cantitate,
            float cantPortie,
            decimal pretUnitate)
            => _repo.AddItemToOrderAsync(comandaId, preparatId, meniuId, cantitate, cantPortie, pretUnitate);

        public Task<(IEnumerable<MeniuDetailDto> Items, MeniuTotalsDto Totals)>
            GetMeniuDetailsAsync(int meniuId)
            => _repo.GetMeniuDetailsAsync(meniuId);

        public Task RemovePreparatFromMeniuAsync(int meniuId, int preparatId)
            => _repo.RemovePreparatFromMeniuAsync(meniuId, preparatId);

        public Task<NewOrderResultDto> CreateOrderWithItemsAsync(
            int utilizatorId,
            decimal discount,
            decimal costTransport,
            DataTable itemsTable)
            => _repo.CreateOrderWithItemsAsync(utilizatorId, discount, costTransport, itemsTable);


    }
}
