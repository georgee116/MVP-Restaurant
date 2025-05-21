// Restaurant.ViewModels/IstoricComenziViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Restaurant.Domain.Entities;
using Restaurant.Services.Services;
using Restaurant.ViewModels.Common;

namespace Restaurant.ViewModels
{
    public class IstoricComenziViewModel : BaseViewModel
    {
        private readonly ComandaService _comandaService;

        // Colecția de comenzi
        public ObservableCollection<Comanda> Comenzi { get; } = new ObservableCollection<Comanda>();

        // Comanda selectată
        private Comanda _selectedComanda;
        public Comanda SelectedComanda
        {
            get => _selectedComanda;
            set
            {
                _selectedComanda = value;
                OnPropertyChanged();
            }
        }

        // Comenzi
        public ICommand LoadComenziCommand { get; }

        // Constructor
        public IstoricComenziViewModel()
        {
            _comandaService = new ComandaService();

            LoadComenziCommand = new RelayCommand(async _ => await LoadComenziAsync());

            // Încarcă comenzile la inițializare
            _ = LoadComenziAsync();
        }

        // Încarcă comenzile utilizatorului curent sau toate comenzile pentru admin
        public async Task LoadComenziAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Încărcare comenzi...");
                Comenzi.Clear();

                // Obținem utilizatorul și rolul curent
                var mainVM = System.Windows.Application.Current.MainWindow?.DataContext as MainViewModel;
                if (mainVM == null)
                {
                    System.Diagnostics.Debug.WriteLine("EROARE: MainViewModel nu este disponibil");
                    return;
                }

                // Verificăm rolul utilizatorului
                if (mainVM.IsAngajat)
                {
                    System.Diagnostics.Debug.WriteLine("Utilizator este angajat, încărcăm toate comenzile");
                    var comenzi = await _comandaService.GetAllComenziAsync();
                    foreach (var c in comenzi)
                    {
                        Comenzi.Add(c);
                    }
                    System.Diagnostics.Debug.WriteLine($"S-au încărcat {Comenzi.Count} comenzi pentru angajat");
                }
                else if (mainVM.CurrentUser != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Utilizator client ID: {mainVM.CurrentUser.Id}, încărcăm comenzile sale");
                    var comenzi = await _comandaService.GetComenziForUserAsync(mainVM.CurrentUser.Id);
                    foreach (var c in comenzi)
                    {
                        Comenzi.Add(c);
                    }
                    System.Diagnostics.Debug.WriteLine($"S-au încărcat {Comenzi.Count} comenzi pentru client");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Utilizator neautentificat, nu se încarcă comenzi");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EROARE la încărcarea comenzilor: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }
    }
}