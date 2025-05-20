using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Data;
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
        private readonly PreparatService _preparatService = new();
        private readonly MeniuService _meniuService = new();

        // Colecția de itemi în comanda curentă
        public ObservableCollection<ComandaItem> Items { get; } = new();

        // Colecții pentru a afișa produsele disponibile în interfață
        public ObservableCollection<Preparat> AllPreparate { get; } = new();
        public ObservableCollection<Meniu> AllMeniuri { get; } = new();

        // Proprietate pentru comanda curentă
        private Comanda _currentOrder;
        public Comanda CurrentOrder
        {
            get => _currentOrder;
            private set
            {
                _currentOrder = value;
                OnPropertyChanged();
                IsComandaActiva = value != null;
                OnPropertyChanged(nameof(IsComandaActiva));
                OnPropertyChanged(nameof(CostTotal));
                RaiseCanExecChanged();
            }
        }

        // Selecții pentru interfață
        private Preparat _selectedPreparat;
        public Preparat SelectedPreparat
        {
            get => _selectedPreparat;
            set
            {
                _selectedPreparat = value;
                OnPropertyChanged();
                ((RelayCommand)AddPreparatCommand).RaiseCanExecuteChanged();
            }
        }

        private Meniu _selectedMeniu;
        public Meniu SelectedMeniu
        {
            get => _selectedMeniu;
            set
            {
                _selectedMeniu = value;
                OnPropertyChanged();
                ((RelayCommand)AddMeniuCommand).RaiseCanExecuteChanged();
            }
        }

        private ComandaItem _selectedItem;
        public ComandaItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                ((RelayCommand)RemoveItemCommand).RaiseCanExecuteChanged();
            }
        }

        // Cantitatea pentru a adăuga preparat/meniu
        private int _cantitate = 1;
        public int Cantitate
        {
            get => _cantitate;
            set
            {
                if (value > 0)
                {
                    _cantitate = value;
                    OnPropertyChanged();
                }
            }
        }

        // Stare comandă
        public bool IsComandaActiva { get; private set; }

        // Cost total
        public decimal CostTotal
        {
            get
            {
                if (CurrentOrder == null) return 0;

                decimal totalProduse = Items.Sum(i => i.PretUnitate * i.Cantitate);
                decimal discount = CurrentOrder.DiscountAplicat;
                decimal transport = CurrentOrder.CostTransport;

                return totalProduse - discount + transport;
            }
        }

        // Evenimente pentru notificări
        public event Action<string> ComandaPlasata;

        // Comenzi UI
        public ICommand StartOrderCommand { get; }
        public ICommand CancelOrderCommand { get; }
        public ICommand AddPreparatCommand { get; }
        public ICommand AddMeniuCommand { get; }
        public ICommand AddMenuToOrderCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand PlaceOrderCommand { get; }
        public ICommand LoadProductsCommand { get; }

        // Constructor
        public ComandaViewModel()
        {
            // Configurare comenzi
            StartOrderCommand = new RelayCommand(_ => StartNewOrder(), _ => !IsComandaActiva);
            CancelOrderCommand = new RelayCommand(_ => CancelOrder(), _ => IsComandaActiva);

            AddPreparatCommand = new RelayCommand(_ => AddPreparat(), _ => IsComandaActiva && SelectedPreparat != null && Cantitate > 0);
            AddMeniuCommand = new RelayCommand(_ => AddMeniu(), _ => IsComandaActiva && SelectedMeniu != null && Cantitate > 0);

            AddMenuToOrderCommand = new RelayCommand(async param =>
            {
                if (!IsComandaActiva) StartNewOrder();
                if (param is Meniu meniu)
                    await AddMeniuToOrder(meniu);
            }, _ => true);

            RemoveItemCommand = new RelayCommand(_ => RemoveItem(), _ => IsComandaActiva && SelectedItem != null);

            PlaceOrderCommand = new RelayCommand(async _ => await PlaceOrderAsync(), _ => IsComandaActiva && Items.Count > 0);

            LoadProductsCommand = new RelayCommand(async _ => await LoadProductsAsync());

            // Incărcăm produsele inițial
            _ = LoadProductsAsync();
        }

        // Inițiere comandă nouă
        private void StartNewOrder()
        {
            CurrentOrder = new Comanda
            {
                DataComenzii = DateTime.Now,
                Status = OrderStatus.Inregistrata,
                DiscountAplicat = 0, // Se va calcula la plasare
                CostTransport = 10.0m, // Implicit, se va actualiza la plasare
                UtilizatorId = GetCurrentUserId() // Trebuie implementată - preia din MainViewModel
            };

            Items.Clear();
        }

        // Anulare comandă
        private void CancelOrder()
        {
            CurrentOrder = null;
            Items.Clear();
        }

        // Adăugare preparat în comandă
        private void AddPreparat()
        {
            if (SelectedPreparat == null || !IsComandaActiva || Cantitate <= 0)
                return;

            // Verificăm disponibilitatea
            if (SelectedPreparat.CantitateTotala < SelectedPreparat.CantitatePortie * Cantitate)
            {
                // Notificare insuficient
                System.Windows.MessageBox.Show($"Cantitate insuficientă disponibilă pentru {SelectedPreparat.Denumire}!",
                    "Stoc insuficient", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            // Verificăm dacă există deja în comandă și actualizăm
            var existingItem = Items.FirstOrDefault(i => i.PreparatId == SelectedPreparat.Id && i.MeniuId == null);
            if (existingItem != null)
            {
                existingItem.Cantitate += Cantitate;
                OnPropertyChanged(nameof(CostTotal));
                return;
            }

            // Altfel adăugăm nou
            Items.Add(new ComandaItem
            {
                PreparatId = SelectedPreparat.Id,
                Preparat = SelectedPreparat,
                MeniuId = null,
                Cantitate = Cantitate,
                CantitatePortie = SelectedPreparat.CantitatePortie,
                PretUnitate = SelectedPreparat.Pret
            });

            OnPropertyChanged(nameof(CostTotal));
        }

        // Adăugare meniu în comandă
        private void AddMeniu()
        {
            if (SelectedMeniu == null || !IsComandaActiva || Cantitate <= 0)
                return;

            // Verificăm dacă există deja în comandă și actualizăm
            var existingItem = Items.FirstOrDefault(i => i.MeniuId == SelectedMeniu.Id && i.PreparatId == null);
            if (existingItem != null)
            {
                existingItem.Cantitate += Cantitate;
                OnPropertyChanged(nameof(CostTotal));
                return;
            }

            // Calculăm prețul
            decimal pretMeniu = CalculatePretMeniu(SelectedMeniu.Id);

            // Adăugăm în comandă
            Items.Add(new ComandaItem
            {
                MeniuId = SelectedMeniu.Id,
                Meniu = SelectedMeniu,
                PreparatId = null,
                Cantitate = Cantitate,
                CantitatePortie = 1, // Pentru meniu este 1 porție
                PretUnitate = pretMeniu
            });

            OnPropertyChanged(nameof(CostTotal));
        }

        // Adăugare meniu din altă parte (ex: tab Meniuri)
        private async Task AddMeniuToOrder(Meniu meniu)
        {
            if (meniu == null || !IsComandaActiva)
                return;

            // Verificăm dacă există deja în comandă și actualizăm
            var existingItem = Items.FirstOrDefault(i => i.MeniuId == meniu.Id && i.PreparatId == null);
            if (existingItem != null)
            {
                existingItem.Cantitate += 1;
                OnPropertyChanged(nameof(CostTotal));
                return;
            }

            // Calculăm prețul
            decimal pretMeniu = CalculatePretMeniu(meniu.Id);

            // Adăugăm în comandă
            Items.Add(new ComandaItem
            {
                MeniuId = meniu.Id,
                Meniu = meniu,
                PreparatId = null,
                Cantitate = 1,
                CantitatePortie = 1, // Pentru meniu este 1 porție
                PretUnitate = pretMeniu
            });

            OnPropertyChanged(nameof(CostTotal));
        }

        // Eliminare item din comandă
        private void RemoveItem()
        {
            if (SelectedItem != null && IsComandaActiva)
            {
                Items.Remove(SelectedItem);
                SelectedItem = null;
                OnPropertyChanged(nameof(CostTotal));
            }
        }

        // Plasare comandă finală
        private async Task PlaceOrderAsync()
        {
            if (!IsComandaActiva || Items.Count == 0)
                return;

            try
            {
                // Calculăm discount conform regulilor (în real ar fi citit din config)
                decimal valoareCumparaturi = Items.Sum(i => i.PretUnitate * i.Cantitate);
                decimal discount = 0;
                decimal limitaDiscount = 100; // Exemplu - ar fi din config

                if (valoareCumparaturi > limitaDiscount)
                {
                    discount = valoareCumparaturi * 0.1m; // 10% discount
                }

                // Calculăm transport
                decimal transport = valoareCumparaturi < 50 ? 10.0m : 0.0m; // Transport gratuit peste 50 lei

                // Actualizăm comanda
                CurrentOrder.DiscountAplicat = discount;
                CurrentOrder.CostTransport = transport;
                CurrentOrder.OraEstimativaLivrare = DateTime.Now.AddHours(1); // Exemplu

                // Creăm DataTable pentru procedura stocată
                var itemsTable = new DataTable();
                itemsTable.Columns.Add("PreparatId", typeof(int));
                itemsTable.Columns.Add("MeniuId", typeof(int));
                itemsTable.Columns.Add("Cantitate", typeof(int));
                itemsTable.Columns.Add("CantPortie", typeof(float));
                itemsTable.Columns.Add("PretUnitate", typeof(decimal));

                foreach (var item in Items)
                {
                    itemsTable.Rows.Add(
                        item.PreparatId.HasValue ? item.PreparatId.Value : DBNull.Value,
                        item.MeniuId.HasValue ? item.MeniuId.Value : DBNull.Value,
                        item.Cantitate,
                        item.CantitatePortie,
                        item.PretUnitate
                    );
                }

                // Apelăm procedura stocată
                var result = await _spService.CreateOrderWithItemsAsync(
                    CurrentOrder.UtilizatorId,
                    CurrentOrder.DiscountAplicat,
                    CurrentOrder.CostTransport,
                    itemsTable
                );

                // Actualizăm comanda cu ID-ul și codul unic
                CurrentOrder.Id = result.ComandaId;
                CurrentOrder.CodUnic = result.CodUnic;

                // Notificăm UI
                ComandaPlasata?.Invoke(CurrentOrder.CodUnic);

                // Reset comanda
                CurrentOrder = null;
                Items.Clear();
                Cantitate = 1;

                System.Windows.MessageBox.Show($"Comanda cu codul {result.CodUnic} a fost plasată cu succes!",
                    "Comandă plasată", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Eroare la plasarea comenzii: {ex.Message}",
                    "Eroare", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        // Încărcare produse
        private async Task LoadProductsAsync()
        {
            try
            {
                // Încărcăm preparatele
                AllPreparate.Clear();
                var preparate = await _preparatService.GetAllAsync();
                foreach (var p in preparate)
                {
                    AllPreparate.Add(p);
                }

                // Încărcăm meniurile
                AllMeniuri.Clear();
                var meniuri = await _meniuService.GetAllAsync();
                foreach (var m in meniuri)
                {
                    AllMeniuri.Add(m);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Eroare la încărcarea produselor: {ex.Message}");
            }
        }

        // Helper pentru a calcula prețul unui meniu
        private decimal CalculatePretMeniu(int meniuId)
        {
            try
            {
                var result = _spService.GetMeniuDetailsAsync(meniuId).GetAwaiter().GetResult();
                decimal pretTotal = result.Totals.TotalPret;

                // Aplicăm reducerea de meniu (ar trebui să fie din config)
                decimal reducereMeniu = 0.15m; // 15% reducere
                return pretTotal * (1 - reducereMeniu);
            }
            catch
            {
                // Fallback - în caz de eroare, returnăm 0
                return 0;
            }
        }

        // Helper pentru utilizatorul curent
        private int GetCurrentUserId()
        {
            // În real, am obține ID-ul utilizatorului din MainViewModel
            // Pentru acum, returnăm 1 (primul utilizator)
            return 1;
        }

        // Helper pentru CanExecuteChanged al comenzilor
        private void RaiseCanExecChanged()
        {
            ((RelayCommand)StartOrderCommand).RaiseCanExecuteChanged();
            ((RelayCommand)CancelOrderCommand).RaiseCanExecuteChanged();
            ((RelayCommand)AddPreparatCommand).RaiseCanExecuteChanged();
            ((RelayCommand)AddMeniuCommand).RaiseCanExecuteChanged();
            ((RelayCommand)RemoveItemCommand).RaiseCanExecuteChanged();
            ((RelayCommand)PlaceOrderCommand).RaiseCanExecuteChanged();
        }
    }
}