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
        // 3) Get menu details + totals
        public async Task<(IEnumerable<MeniuDetailDto> Items, MeniuTotalsDto Totals)> GetMeniuDetailsAsync(int meniuId)
        {
            using var ctx = new ApplicationDbContext();
            var items = new List<MeniuDetailDto>();
            MeniuTotalsDto totals = new MeniuTotalsDto();

            System.Diagnostics.Debug.WriteLine($"Apel procedură stocată pentru meniul cu ID: {meniuId}");

            try
            {
                var connection = ctx.Database.GetDbConnection();
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "EXEC dbo.usp_GetMeniuDetails @MeniuId";
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@MeniuId";
                    parameter.Value = meniuId;
                    command.Parameters.Add(parameter);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        System.Diagnostics.Debug.WriteLine("Preparate în meniu:");
                        int count = 0;

                        while (await reader.ReadAsync())
                        {
                            count++;
                            var item = new MeniuDetailDto
                            {
                                MeniuId = Convert.ToInt32(reader["MeniuId"]),
                                Denumire = reader["Denumire"].ToString(),
                                Categorie = reader["Categorie"].ToString(),
                                PreparatId = Convert.ToInt32(reader["PreparatId"]),
                                Preparat = reader["Preparat"].ToString(),
                                GramajPortie = Convert.ToSingle(reader["GramajPortie"]),
                                PretStandard = Convert.ToDecimal(reader["PretStandard"]),
                                Subtotal = Convert.ToDecimal(reader["Subtotal"])
                            };

                            items.Add(item);

                            System.Diagnostics.Debug.WriteLine($"  {count}. Preparat: {item.Preparat}, GramajPortie: {item.GramajPortie}, PretStandard: {item.PretStandard}, Subtotal: {item.Subtotal}");
                        }

                        System.Diagnostics.Debug.WriteLine($"Total preparate găsite: {count}");

                        // Trecem la următorul set de rezultate (totals)
                        if (await reader.NextResultAsync() && await reader.ReadAsync())
                        {
                            System.Diagnostics.Debug.WriteLine("Totals:");

                            totals = new MeniuTotalsDto
                            {
                                TotalGramaj = Convert.ToSingle(reader["TotalGramaj"]),
                                TotalPret = Convert.ToDecimal(reader["TotalPret"])
                            };

                            System.Diagnostics.Debug.WriteLine($"  TotalGramaj: {totals.TotalGramaj}, TotalPret: {totals.TotalPret}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Nu există al doilea set de rezultate (Totals)");
                        }
                    }
                }

                return (items, totals);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Eroare la apelul procedurii stocate: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                return (new List<MeniuDetailDto>(), new MeniuTotalsDto { TotalGramaj = 0, TotalPret = 0 });
            }
        }

        // 4) Remove a preparat from a menu
        public Task RemovePreparatFromMeniuAsync(int meniuId, int preparatId)
        {
            using var ctx = new ApplicationDbContext();
            // Asigură-te că conexiunea este deschisă înainte de executare
            if (ctx.Database.GetDbConnection().State != System.Data.ConnectionState.Open)
            {
                ctx.Database.GetDbConnection().Open();
            }

            return ctx.Database.ExecuteSqlRawAsync(
                "EXEC dbo.usp_RemovePreparatFromMeniu @MeniuId, @PreparatId",
                new SqlParameter("@MeniuId", meniuId),
                new SqlParameter("@PreparatId", preparatId)
            );
        }

        // 5) Create order + items in one go (TVP)
        // În StoredProceduresRepository.cs, metoda CreateOrderWithItemsAsync
        // În StoredProceduresRepository.cs
        // În StoredProceduresRepository.cs

        // Adăugați aceste using-uri


        public async Task<NewOrderResultDto> CreateOrderWithItemsAsync(
        int utilizatorId,
        decimal discount,
        decimal costTransport,
        DataTable itemsTable)
        {
            using var ctx = new ApplicationDbContext();
            var result = new NewOrderResultDto();

            try
            {
                var connection = ctx.Database.GetDbConnection();
                await connection.OpenAsync();

                // Specificăm că folosim SqlCommand, nu DbCommand
                using (var command = connection.CreateCommand() as SqlCommand)
                {
                    if (command == null)
                        throw new InvalidOperationException("Conexiunea nu este pentru SQL Server");

                    command.CommandText = "EXEC dbo.usp_CreateOrderWithItems @UtilizatorId, @Discount, @CostTransport, @TVP_Items";

                    // Parametru 1: UtilizatorId
                    command.Parameters.Add(new SqlParameter("@UtilizatorId", utilizatorId));

                    // Parametru 2: Discount
                    command.Parameters.Add(new SqlParameter("@Discount", discount));

                    // Parametru 3: CostTransport
                    command.Parameters.Add(new SqlParameter("@CostTransport", costTransport));

                    // Parametru 4: TVP_Items (Table-Valued Parameter)
                    var paramTVP = new SqlParameter
                    {
                        ParameterName = "@TVP_Items",
                        Value = itemsTable,
                        SqlDbType = SqlDbType.Structured,
                        TypeName = "dbo.TVP_ComandaItem"
                    };
                    command.Parameters.Add(paramTVP);

                    // Executăm procedura stocată și citim rezultatul
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            result.ComandaId = reader.GetInt32(reader.GetOrdinal("ComandaId"));
                            result.CodUnic = reader.GetString(reader.GetOrdinal("CodUnic"));
                        }
                        else
                        {
                            throw new Exception("Procedura stocată nu a returnat niciun rezultat");
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EROARE în CreateOrderWithItemsAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}