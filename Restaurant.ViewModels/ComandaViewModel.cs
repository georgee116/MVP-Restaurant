using Restaurant.Data.Context;
using Restaurant.Domain;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.Services.Interfaces;
using Restaurant.Services.Services;
using Restaurant.ViewModels.Common;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace Restaurant.ViewModels
{
    public class ComandaViewModel : BaseViewModel
    {
        private readonly StoredProceduresService _spService = new();
        private readonly PreparatService _preparatService = new();
        private readonly MeniuService _meniuService = new();
        private readonly PreparatMeniuService _preparatMeniuService = new();


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
                if (param is Meniu meniu)
                {
                    // Pentru debugging
                    System.Diagnostics.Debug.WriteLine($"Adaugă în comandă meniul: {meniu.Denumire} (ID: {meniu.Id})");

                    // Verificăm dacă există o comandă activă
                    if (!IsComandaActiva)
                    {
                        StartNewOrder();
                    }

                    // Adăugăm meniul în comandă
                    await AddMeniuToOrder(meniu);
                }
            }, _ => true);

            RemoveItemCommand = new RelayCommand(_ => RemoveItem(), _ => IsComandaActiva && SelectedItem != null);

            // În constructor sau unde sunt inițializate comenzile
            // În constructor sau unde sunt inițializate comenzile în ComandaViewModel.cs
            PlaceOrderCommand = new RelayCommand(async _ =>
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("PlaceOrderCommand executat");
                    await PlaceOrderAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Eroare în PlaceOrderCommand: {ex.Message}");
                    System.Windows.MessageBox.Show($"Eroare la plasarea comenzii: {ex.Message}",
                        "Eroare", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }, _ => true); // Am schimbat condiția pentru a permite întotdeauna executarea

            LoadProductsCommand = new RelayCommand(async _ => await LoadProductsAsync());

            // Incărcăm produsele inițial
            _ = LoadProductsAsync();
        }

        // Inițiere comandă nouă
       
        // Anulare comandă
        private void CancelOrder()
        {
            CurrentOrder = null;
            Items.Clear();
        }

        // Adăugare preparat în comandă
        private async Task AddPreparat()
        {
            if (SelectedPreparat == null || !IsComandaActiva || Cantitate <= 0)
                return;

            // Calculează cantitatea necesară
            float cantitateNecesara = SelectedPreparat.CantitatePortie * Cantitate;

            // Verifică disponibilitatea
            bool stocDisponibil = await _preparatService.VerificaStocDisponibilAsync(SelectedPreparat.Id, cantitateNecesara);
            if (!stocDisponibil)
            {
                float portiiDisponibile = (float)Math.Floor(SelectedPreparat.CantitateTotala / SelectedPreparat.CantitatePortie);
                if (portiiDisponibile <= 0)
                {
                    System.Windows.MessageBox.Show($"Ne pare rău, produsul '{SelectedPreparat.Denumire}' nu este disponibil în stoc!",
                        "Stoc epuizat", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }
                else
                {
                    var result = System.Windows.MessageBox.Show(
                        $"Cantitatea solicitată ({Cantitate} porții) nu este disponibilă în stoc.\n" +
                        $"Sunt disponibile doar {portiiDisponibile} porții.\n" +
                        $"Doriți să adăugați cantitatea maximă disponibilă?",
                        "Stoc insuficient",
                        System.Windows.MessageBoxButton.YesNo,
                        System.Windows.MessageBoxImage.Question);

                    if (result == System.Windows.MessageBoxResult.Yes)
                    {
                        Cantitate = (int)portiiDisponibile;
                    }
                    else
                    {
                        return; // Utilizatorul anulează
                    }
                }
            }
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
        // În ComandaViewModel.cs
        // Modificăm AddMeniuToOrder pentru a verifica disponibilitatea
        // În Restaurant.ViewModels/ComandaViewModel.cs
        private async Task AddMeniuToOrder(Meniu meniu)
        {
            if (meniu == null || !IsComandaActiva)
            {
                System.Diagnostics.Debug.WriteLine("Nu se poate adăuga în comandă: meniu null sau comandă inactivă");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"Verifică disponibilitatea pentru meniul: {meniu.Denumire}");

            try
            {
                // Verificăm disponibilitatea tuturor preparatelor din meniu
                var preparateMeniu = await _preparatMeniuService.GetPreparatMeniuriAsync(meniu.Id);
                bool allAvailable = true;
                string unavailablePreparate = "";

                foreach (var pm in preparateMeniu)
                {
                    var preparat = await _preparatService.GetByIdAsync(pm.PreparatId);
                    if (preparat == null || preparat.CantitateTotala < pm.CantitatePortieMeniu)
                    {
                        allAvailable = false;
                        unavailablePreparate += (preparat?.Denumire ?? "Preparat necunoscut") + ", ";
                    }
                }

                if (!allAvailable)
                {
                    System.Diagnostics.Debug.WriteLine("Unele preparate nu sunt disponibile");

                    if (unavailablePreparate.EndsWith(", "))
                        unavailablePreparate = unavailablePreparate.Substring(0, unavailablePreparate.Length - 2);

                    System.Windows.MessageBox.Show(
                        $"Meniul '{meniu.Denumire}' nu poate fi adăugat în comandă deoarece următoarele preparate nu sunt disponibile: {unavailablePreparate}",
                        "Stoc insuficient",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Warning);
                    return;
                }

                // Verificăm dacă există deja în comandă și actualizăm
                var existingItem = Items.FirstOrDefault(i => i.MeniuId == meniu.Id && i.PreparatId == null);
                if (existingItem != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Meniul este deja în comandă. Actualizăm cantitatea la {existingItem.Cantitate + 1}");
                    existingItem.Cantitate += 1;
                    OnPropertyChanged(nameof(CostTotal));

                    System.Windows.MessageBox.Show(
                        $"Cantitatea pentru meniul '{meniu.Denumire}' a fost actualizată.",
                        "Comandă actualizată",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Information);
                    return;
                }

                // Calculăm prețul meniului
                decimal pretMeniu = CalculatePretMeniu(meniu.Id);
                System.Diagnostics.Debug.WriteLine($"Preț calculat pentru meniu: {pretMeniu}");

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

                System.Windows.MessageBox.Show(
                    $"Meniul '{meniu.Denumire}' a fost adăugat în comandă.",
                    "Adăugat cu succes",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Eroare la adăugarea în comandă: {ex.Message}");
                System.Windows.MessageBox.Show(
                    $"A apărut o eroare la adăugarea meniului în comandă: {ex.Message}",
                    "Eroare",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        // Modificăm PlaceOrderAsync pentru a scădea stocul
        private async Task PlaceOrderAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Începere PlaceOrderAsync");

                if (!IsComandaActiva || Items.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Nu se poate plasa comanda: comanda inactivă sau fără itemi");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Itemi în comandă: {Items.Count}");

                // Calculăm discount conform regulilor (în real ar fi citit din config)
                decimal valoareCumparaturi = Items.Sum(i => i.PretUnitate * i.Cantitate);
                decimal discount = 0;
                decimal limitaDiscount = AppSettings.Instance.OrderValueDiscountLimit; // Exemplu - ar fi din config

                System.Diagnostics.Debug.WriteLine($"Valoare cumpărături: {valoareCumparaturi}, limită discount: {limitaDiscount}");

                if (valoareCumparaturi > limitaDiscount)
                {
                    discount = valoareCumparaturi * AppSettings.Instance.OrderValueDiscountPercent;
                    System.Diagnostics.Debug.WriteLine($"Discount aplicat: {discount}");
                }

                // Calculăm transport
                decimal transport = valoareCumparaturi < AppSettings.Instance.TransportPriceLimitFree ?
                                    AppSettings.Instance.TransportPrice : 0.0m; // Transport gratuit peste limită

                System.Diagnostics.Debug.WriteLine($"Cost transport: {transport}");

                // Actualizăm comanda
                CurrentOrder.DiscountAplicat = discount;
                CurrentOrder.CostTransport = transport;
                CurrentOrder.OraEstimativaLivrare = DateTime.Now.AddHours(1); // Exemplu

                // Verificăm utilizatorul curent
                System.Diagnostics.Debug.WriteLine($"ID Utilizator: {CurrentOrder.UtilizatorId}");

                // Creăm DataTable pentru procedura stocată
                var itemsTable = new DataTable();
                itemsTable.Columns.Add("PreparatId", typeof(int));
                itemsTable.Columns.Add("MeniuId", typeof(int));
                itemsTable.Columns.Add("Cantitate", typeof(int));
                itemsTable.Columns.Add("CantPortie", typeof(float));
                itemsTable.Columns.Add("PretUnitate", typeof(decimal));

                foreach (var item in Items)
                {
                    var row = itemsTable.NewRow();
                    row["PreparatId"] = item.PreparatId ?? (object)DBNull.Value;
                    row["MeniuId"] = item.MeniuId ?? (object)DBNull.Value;
                    row["Cantitate"] = item.Cantitate;
                    row["CantPortie"] = item.CantitatePortie;
                    row["PretUnitate"] = item.PretUnitate;
                    itemsTable.Rows.Add(row);

                    System.Diagnostics.Debug.WriteLine($"Item adăugat în tabel: PreparatId={item.PreparatId}, MeniuId={item.MeniuId}, Cantitate={item.Cantitate}");
                }

                // Verificăm datele înainte de a trimite la procedura stocată
                System.Diagnostics.Debug.WriteLine($"Trimit la procedură stocată: UtilizatorId={CurrentOrder.UtilizatorId}, Discount={CurrentOrder.DiscountAplicat}, Transport={CurrentOrder.CostTransport}, Itemi={itemsTable.Rows.Count}");

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

                System.Diagnostics.Debug.WriteLine($"Comandă creată: ID={result.ComandaId}, Cod={result.CodUnic}");

                // Scădem stocul pentru fiecare item comandat
                foreach (var item in Items)
                {
                    if (item.PreparatId.HasValue)
                    {
                        // Este un preparat individual
                        float cantitateConsumata = item.CantitatePortie * item.Cantitate;
                        await _preparatService.ScadeStocAsync(item.PreparatId.Value, cantitateConsumata);
                        System.Diagnostics.Debug.WriteLine($"Stoc scăzut pentru preparat ID={item.PreparatId} cu {cantitateConsumata}");
                    }
                    else if (item.MeniuId.HasValue)
                    {
                        // Este un meniu - scade stocul pentru fiecare preparat din meniu
                        var preparateMeniu = await _preparatMeniuService.GetPreparatMeniuriAsync(item.MeniuId.Value);
                        foreach (var pm in preparateMeniu)
                        {
                            float cantitateConsumata = pm.CantitatePortieMeniu * item.Cantitate;
                            await _preparatService.ScadeStocAsync(pm.PreparatId, cantitateConsumata);
                            System.Diagnostics.Debug.WriteLine($"Stoc scăzut pentru preparat ID={pm.PreparatId} din meniu ID={item.MeniuId} cu {cantitateConsumata}");
                        }
                    }
                }

                // Notificăm UI
                ComandaPlasata?.Invoke(CurrentOrder.CodUnic);

                System.Diagnostics.Debug.WriteLine("Comandă finalizată cu succes!");

                ComandaPlasata?.Invoke(CurrentOrder.CodUnic);

                // Reset comanda
                CurrentOrder = null;
                Items.Clear();
                IsComandaActiva = false;
                OnPropertyChanged(nameof(IsComandaActiva));
                OnPropertyChanged(nameof(CurrentOrder));
                OnPropertyChanged(nameof(CostTotal));

                System.Windows.MessageBox.Show($"Comanda cu codul {result.CodUnic} a fost plasată cu succes!",
                    "Comandă plasată", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EROARE la plasarea comenzii: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");

                System.Windows.MessageBox.Show($"Eroare la plasarea comenzii: {ex.Message}",
                    "Eroare", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
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

        // În Restaurant.ViewModels/ComandaViewModel.cs
        // În Restaurant.ViewModels/ComandaViewModel.cs
        private int GetCurrentUserId()
        {
            // Verificăm dacă utilizatorul curent este setat
            var mainVM = (System.Windows.Application.Current.MainWindow?.DataContext as MainViewModel);
            if (mainVM?.CurrentUser != null)
            {
                System.Diagnostics.Debug.WriteLine($"Utilizator curent: {mainVM.CurrentUser.Nume} {mainVM.CurrentUser.Prenume}, ID: {mainVM.CurrentUser.Id}");
                return mainVM.CurrentUser.Id;
            }

            // Valoare default - poate fi cauza problemei!
            System.Diagnostics.Debug.WriteLine("ATENȚIE: Se folosește ID de utilizator default (1)!");
            return 1; // Verificați dacă acest utilizator există în baza de date
        }

        // Plasare comandă finală


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
        // În Restaurant.ViewModels/ComandaViewModel.cs
        private decimal CalculatePretMeniu(int meniuId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Calculez prețul pentru meniul cu ID: {meniuId}");

                using var ctx = new ApplicationDbContext();

                // Calculăm suma prețurilor preparatelor din meniu
                var pretTotal = ctx.PreparatMeniuri
                    .Where(pm => pm.MeniuId == meniuId)
                    .Join(ctx.Preparate, pm => pm.PreparatId, p => p.Id, (pm, p) => new { pm, p })
                    .Sum(x => (decimal)x.pm.CantitatePortieMeniu * x.p.Pret); // Convertim float la decimal

                // Aplicăm reducerea de meniu (15% din AppSettings)
                decimal reducereMeniu = AppSettings.Instance.MeniuDiscount; // de exemplu 0.15m

                System.Diagnostics.Debug.WriteLine($"Preț total înainte de reducere: {pretTotal}, Reducere: {reducereMeniu * 100}%");

                return pretTotal * (1 - reducereMeniu);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Eroare la calcularea prețului meniului: {ex.Message}");
                return 0;
            }
        }



        
        
        // Redenumim metoda noastră
        private int GetCurrentUserIdFromMainVM()
        {
            // Aici ar trebui să obțineți ID-ul utilizatorului curent din MainViewModel
            var mainVM = (Application.Current.MainWindow?.DataContext as MainViewModel);
            if (mainVM?.CurrentUser != null)
                return mainVM.CurrentUser.Id;

            // Valoare default pentru testare
            return 1;
        }

        // Și actualizăm apelul din StartNewOrder
        private void StartNewOrder()
        {
            System.Diagnostics.Debug.WriteLine("Se creează o comandă nouă");

            CurrentOrder = new Comanda
            {
                DataComenzii = DateTime.Now,
                Status = OrderStatus.Inregistrata,
                DiscountAplicat = 0,
                CostTransport = 10.0m,
                UtilizatorId = GetCurrentUserIdFromMainVM() // Folosim noua metodă
            };

            Items.Clear();

            IsComandaActiva = true;
            OnPropertyChanged(nameof(IsComandaActiva));
            OnPropertyChanged(nameof(CurrentOrder));
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