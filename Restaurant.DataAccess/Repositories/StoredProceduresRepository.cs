// Restaurant.DataAccess/Repositories/StoredProceduresRepository.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data.Context;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Entities;

namespace Restaurant.DataAccess.Repositories
{
    public class StoredProceduresRepository
    {
        // 1) List all allergens
        public async Task<IEnumerable<Alergen>> GetAllAlergeniAsync()
        {
            using var ctx = new ApplicationDbContext();
            return await ctx.Alergeni
                            .FromSqlRaw("EXEC dbo.usp_GetAllAlergeni")
                            .ToListAsync();
        }

        // 2) Add an item to an existing order
        public Task AddItemToOrderAsync(
            int comandaId,
            int preparatId,
            int? meniuId,
            int cantitate,
            float cantPortie,
            decimal pretUnitate)
        {
            using var ctx = new ApplicationDbContext();
            return ctx.Database.ExecuteSqlRawAsync(
                "EXEC dbo.usp_AddItemToOrder @ComandaId, @PreparatId, @MeniuId, @Cantitate, @CantPortie, @PretUnitate",
                new SqlParameter("@ComandaId", comandaId),
                new SqlParameter("@PreparatId", preparatId),
                new SqlParameter("@MeniuId", (object)meniuId ?? DBNull.Value),
                new SqlParameter("@Cantitate", cantitate),
                new SqlParameter("@CantPortie", cantPortie),
                new SqlParameter("@PretUnitate", pretUnitate)
            );
        }

        // 3) Get menu details + totals
        public async Task<(IEnumerable<MeniuDetailDto> Items, MeniuTotalsDto Totals)> GetMeniuDetailsAsync(int meniuId)
        {
            using var ctx = new ApplicationDbContext();

            var items = await ctx.Set<MeniuDetailDto>()
                                 .FromSqlRaw("EXEC dbo.usp_GetMeniuDetails @MeniuId",
                                             new SqlParameter("@MeniuId", meniuId))
                                 .ToListAsync();

            var totals = await ctx.Set<MeniuTotalsDto>()
                                  .FromSqlRaw("EXEC dbo.usp_GetMeniuDetails @MeniuId",
                                              new SqlParameter("@MeniuId", meniuId))
                                  .FirstOrDefaultAsync();

            return (items, totals!);
        }

        // 4) Remove a preparat from a menu
        public Task RemovePreparatFromMeniuAsync(int meniuId, int preparatId)
        {
            using var ctx = new ApplicationDbContext();
            return ctx.Database.ExecuteSqlRawAsync(
                "EXEC dbo.usp_RemovePreparatFromMeniu @MeniuId, @PreparatId",
                new SqlParameter("@MeniuId", meniuId),
                new SqlParameter("@PreparatId", preparatId)
            );
        }

        // 5) Create order + items in one go (TVP)
        public async Task<NewOrderResultDto> CreateOrderWithItemsAsync(
            int utilizatorId,
            decimal discount,
            decimal costTransport,
            DataTable itemsTable)
        {
            using var ctx = new ApplicationDbContext();
            var parameters = new[]
            {
                new SqlParameter("@UtilizatorId",  utilizatorId),
                new SqlParameter("@Discount",      discount),
                new SqlParameter("@CostTransport", costTransport),
                new SqlParameter
                {
                    ParameterName = "@TVP_Items",
                    SqlDbType      = SqlDbType.Structured,
                    TypeName       = "dbo.TVP_ComandaItem",
                    Value          = itemsTable
                }
            };

            var result = await ctx.Set<NewOrderResultDto>()
                                  .FromSqlRaw("EXEC dbo.usp_CreateOrderWithItems @UtilizatorId, @Discount, @CostTransport, @TVP_Items",
                                               parameters)
                                  .FirstAsync();

            return result;
        }
    }
}
