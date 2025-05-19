using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.Services.Services;
using Restaurant.ViewModels.Common;

namespace Restaurant.ViewModels
{
    public class ComandaViewModel : BaseViewModel
    {
        private readonly StoredProceduresService _spService = new();
        private readonly MeniuService _meniuService = new();

        public ObservableCollection<ComandaItem> Items { get; } = new();
        public Comanda? CurrentOrder { get; private set; }

        // 1) Comanda începe cu această comandă
        public ICommand StartOrderCommand { get; }

        // 2) Adaugă un meniu întreg în comandă (param = Meniu)
        public ICommand AddMenuToOrderCommand { get; }

        // 3) Trimite comanda către SP
        public ICommand PlaceOrderCommand { get; }

        public ComandaViewModel()
        {
            StartOrderCommand = new RelayCommand(_ =>
            {
                CurrentOrder = new Comanda
                {
                    UtilizatorId = 0, // setează după nevoie
                    DataComenzii = DateTime.Now
                };
                Items.Clear();
                RaiseCanExec();
            }, _ => CurrentOrder == null);

            AddMenuToOrderCommand = new RelayCommand(async param =>
            {
                if (CurrentOrder == null || param is not Meniu meniu)
                    return;

                // 1) ia detaliile meniului
                var (list, totals) = await _spService.GetMeniuDetailsAsync(meniu.Id);

                // 2) adaugă fiecare preparat ca item în comandă
                foreach (var dto in list)
                {
                    Items.Add(new ComandaItem
                    {
                        PreparatId = dto.PreparatId,
                        MeniuId = dto.MeniuId,
                        Cantitate = 1,
                        CantitatePortie = dto.GramajPortie,
                        PretUnitate = dto.Subtotal
                    });
                }
                RaiseCanExec();
            }, _ => CurrentOrder != null);

            PlaceOrderCommand = new RelayCommand(async _ =>
            {
                if (CurrentOrder == null || !Items.Any()) return;

                // construiește TVP-ul pentru SP
                var table = new System.Data.DataTable();
                table.Columns.Add("PreparatId", typeof(int));
                table.Columns.Add("MeniuId", typeof(int));
                table.Columns.Add("Cantitate", typeof(int));
                table.Columns.Add("CantPortie", typeof(float));
                table.Columns.Add("PretUnitate", typeof(decimal));

                foreach (var it in Items)
                    table.Rows.Add(it.PreparatId, it.MeniuId, it.Cantitate, it.CantitatePortie, it.PretUnitate);

                // apelează SP-ul
                var result = await _spService.CreateOrderWithItemsAsync(
                    CurrentOrder.UtilizatorId,
                    CurrentOrder.DiscountAplicat,
                    CurrentOrder.CostTransport,
                    table);

                // actualizează comanda
                CurrentOrder.Id = result.ComandaId;
                CurrentOrder.CodUnic = result.CodUnic;

                // reset
                CurrentOrder = null;
                Items.Clear();
                RaiseCanExec();
            }, _ => CurrentOrder != null && Items.Any());
        }

        private void RaiseCanExec()
        {
            ((RelayCommand)StartOrderCommand).RaiseCanExecuteChanged();
            ((RelayCommand)AddMenuToOrderCommand).RaiseCanExecuteChanged();
            ((RelayCommand)PlaceOrderCommand).RaiseCanExecuteChanged();
        }
    }
}
