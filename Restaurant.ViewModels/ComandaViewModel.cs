// Restaurant.ViewModels/ComandaViewModel.cs
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Restaurant.Domain.Entities;
using Restaurant.Domain.DTOs;
using Restaurant.Services.Services;
using Restaurant.ViewModels.Common;

namespace Restaurant.ViewModels
{
    public class ComandaViewModel : BaseViewModel
    {
        private readonly StoredProceduresService _spService = new();
        private readonly ComandaService _cService = new();

        public ObservableCollection<ComandaItem> Items { get; } = new();
        public ObservableCollection<Preparat> AllPreparate { get; } = new();
        public ObservableCollection<Meniu> AllMeniuri { get; } = new();

        private int _utilizatorId;
        public int UtilizatorId
        {
            get => _utilizatorId;
            set { _utilizatorId = value; OnPropertyChanged(); }
        }

        private Comanda? _currentOrder;
        public Comanda? CurrentOrder
        {
            get => _currentOrder;
            set { _currentOrder = value; OnPropertyChanged(); }
        }

        public ICommand StartOrderCommand { get; }
        public ICommand AddItemCommand { get; }
        public ICommand PlaceOrderCommand { get; }

        public ComandaViewModel()
        {
            StartOrderCommand = new RelayCommand(_ =>
            {
                CurrentOrder = new Comanda { UtilizatorId = UtilizatorId, DataComenzii = DateTime.Now };
                Items.Clear();
            }, _ => UtilizatorId > 0);

            AddItemCommand = new RelayCommand(async param =>
            {
                // param poate fi Preparat sau Meniu cu cantitate
                // pentru simplitate, presupunem că Items conţine deja Cantitate şi PretUnitate
                if (CurrentOrder == null) return;
                Items.Add((ComandaItem)param!);
            }, _ => CurrentOrder != null);

            PlaceOrderCommand = new RelayCommand(async _ =>
            {
                if (CurrentOrder == null) return;
                // construieşte un TVP
                var table = new DataTable();
                table.Columns.Add("PreparatId", typeof(int));
                table.Columns.Add("MeniuId", typeof(int));
                table.Columns.Add("Cantitate", typeof(int));
                table.Columns.Add("CantPortie", typeof(float));
                table.Columns.Add("PretUnitate", typeof(decimal));

                foreach (var it in Items)
                    table.Rows.Add(it.PreparatId, it.MeniuId, it.Cantitate, it.CantitatePortie, it.PretUnitate);

                var result = await _spService.CreateOrderWithItemsAsync(
                    CurrentOrder.UtilizatorId,
                    CurrentOrder.DiscountAplicat,
                    CurrentOrder.CostTransport,
                    table);

                // populează CurrentOrder cu id şi cod
                CurrentOrder.Id = result.ComandaId;
                CurrentOrder.CodUnic = result.CodUnic;
            }, _ => CurrentOrder != null && Items.Any());

            // preload preparate şi meniuri
            Task.Run(async () =>
            {
                var sp = new PreparatService();
                foreach (var p in await sp.GetAllAsync()) AllPreparate.Add(p);
                var ms = new MeniuService();
                foreach (var m in await ms.GetAllAsync()) AllMeniuri.Add(m);
            });
        }
    }
}
