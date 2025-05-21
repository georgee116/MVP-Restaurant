// Restaurant.ViewModels/MeniuAdminViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using Restaurant.Domain.Entities;
using Restaurant.Services.Services;
using Restaurant.ViewModels.Common;

namespace Restaurant.ViewModels
{
    public class MeniuAdminViewModel : BaseViewModel
    {
        // Services
        private readonly MeniuService _meniuService = new();
        private readonly CategorieService _categorieService = new();
        private readonly PreparatService _preparatService = new();
        private readonly PreparatMeniuService _preparatMeniuService = new();
        private readonly StoredProceduresService _spService = new();

        // Collections
        public ObservableCollection<Meniu> AllMeniuri { get; } = new();
        public ObservableCollection<Categorie> Categorii { get; } = new();
        public ObservableCollection<Preparat> AllPreparate { get; } = new();
        public ObservableCollection<Preparat> PreparateInMeniu { get; } = new();
        public ObservableCollection<Preparat> PreparateDisponibile { get; } = new();

        // Selected items
        private Meniu _selectedMeniu;
        public Meniu SelectedMeniu
        {
            get => _selectedMeniu;
            set
            {
                _selectedMeniu = value;
                OnPropertyChanged();

                // Important: încărcăm preparatele când se selectează un meniu
                if (value != null)
                {
                    LoadMeniuPreparate();
                }
                else
                {
                    PreparateInMeniu.Clear();
                    PreparateDisponibile.Clear();
                }

                ((RelayCommand)UpdateCommand).RaiseCanExecuteChanged();
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
            }
        }

        private Preparat _selectedPreparatToAdd;
        public Preparat SelectedPreparatToAdd
        {
            get => _selectedPreparatToAdd;
            set
            {
                _selectedPreparatToAdd = value;
                OnPropertyChanged();
                ((RelayCommand)AddPreparatToMeniuCommand).RaiseCanExecuteChanged();
            }
        }

        private Preparat _selectedPreparatToRemove;
        public Preparat SelectedPreparatToRemove
        {
            get => _selectedPreparatToRemove;
            set
            {
                _selectedPreparatToRemove = value;
                OnPropertyChanged();
                ((RelayCommand)RemovePreparatFromMeniuCommand).RaiseCanExecuteChanged();
            }
        }

        // New meniu properties
        private string _newMeniuName = string.Empty;
        public string NewMeniuName
        {
            get => _newMeniuName;
            set
            {
                _newMeniuName = value;
                OnPropertyChanged();
                ((RelayCommand)AddCommand).RaiseCanExecuteChanged();
            }
        }

        private Categorie _selectedNewMeniuCategorie;
        public Categorie SelectedNewMeniuCategorie
        {
            get => _selectedNewMeniuCategorie;
            set
            {
                _selectedNewMeniuCategorie = value;
                OnPropertyChanged();
                ((RelayCommand)AddCommand).RaiseCanExecuteChanged();
            }
        }

        // Quantity for adding preparat to meniu
        private float _cantitatePortieMeniu = 0;
        public float CantitatePortieMeniu
        {
            get => _cantitatePortieMeniu;
            set
            {
                _cantitatePortieMeniu = value;
                OnPropertyChanged();
                ((RelayCommand)AddPreparatToMeniuCommand).RaiseCanExecuteChanged();
            }
        }

        // Commands
        public ICommand LoadCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand AddPreparatToMeniuCommand { get; }
        public ICommand RemovePreparatFromMeniuCommand { get; }

        // Constructor
        public MeniuAdminViewModel()
        {
            // Setup commands
            LoadCommand = new RelayCommand(async _ => await LoadDataAsync());
            AddCommand = new RelayCommand(async _ => await AddMeniuAsync(), _ => true); // Eliminăm condiția pentru a testa
            UpdateCommand = new RelayCommand(async _ => await UpdateMeniuAsync(), _ => SelectedMeniu != null);
            DeleteCommand = new RelayCommand(async _ => await DeleteMeniuAsync(), _ => SelectedMeniu != null);

            AddPreparatToMeniuCommand = new RelayCommand(
                async _ => await AddPreparatToMeniuAsync(),
                _ => SelectedMeniu != null && SelectedPreparatToAdd != null && CantitatePortieMeniu > 0);

            RemovePreparatFromMeniuCommand = new RelayCommand(
                async _ => await RemovePreparatFromMeniuAsync(),
                _ => SelectedMeniu != null && SelectedPreparatToRemove != null);

            // Load initial data
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                // Load categorii
                Categorii.Clear();
                var categorii = await _categorieService.GetAllAsync();
                foreach (var c in categorii)
                    Categorii.Add(c);

                // Load meniuri
                AllMeniuri.Clear();
                var meniuri = await _meniuService.GetAllAsync();
                foreach (var m in meniuri)
                    AllMeniuri.Add(m);

                // Load preparate
                AllPreparate.Clear();
                var preparate = await _preparatService.GetAllAsync();
                foreach (var p in preparate)
                    AllPreparate.Add(p);

                System.Diagnostics.Debug.WriteLine($"Loaded {preparate.Count()} preparate");

                // Reload preparate for selected meniu if exists
                if (SelectedMeniu != null)
                {
                    LoadMeniuPreparate();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        private bool CanAddMeniu(object _)
        {
            return !string.IsNullOrWhiteSpace(NewMeniuName) && SelectedNewMeniuCategorie != null;
        }

        private async Task AddMeniuAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NewMeniuName) || SelectedNewMeniuCategorie == null)
                {
                    System.Windows.MessageBox.Show("Introduceți denumirea meniului și selectați o categorie!",
                        "Date incomplete", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Adăugare meniu: {NewMeniuName}, Categorie: {SelectedNewMeniuCategorie.Nume}");

                var meniu = new Meniu
                {
                    Denumire = NewMeniuName,
                    CategorieId = SelectedNewMeniuCategorie.Id
                };

                await _meniuService.AddAsync(meniu);
                System.Diagnostics.Debug.WriteLine($"Meniu adăugat cu ID: {meniu.Id}");

                // Reîncărcăm lista de meniuri
                await LoadDataAsync();

                // Reset inputs
                NewMeniuName = string.Empty;
                SelectedNewMeniuCategorie = null;

                // Notificare
                System.Windows.MessageBox.Show($"Meniul '{meniu.Denumire}' a fost adăugat cu succes!",
                    "Succes", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding meniu: {ex.Message}");
                System.Windows.MessageBox.Show($"Eroare la adăugarea meniului: {ex.Message}",
                    "Eroare", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async Task UpdateMeniuAsync()
        {
            try
            {
                if (SelectedMeniu == null)
                    return;

                await _meniuService.UpdateAsync(SelectedMeniu);

                // Refresh the list
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating meniu: {ex.Message}");
            }
        }

        private async Task DeleteMeniuAsync()
        {
            try
            {
                if (SelectedMeniu == null)
                    return;

                await _meniuService.DeleteAsync(SelectedMeniu.Id);
                AllMeniuri.Remove(SelectedMeniu);
                SelectedMeniu = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting meniu: {ex.Message}");
            }
        }

        private async void LoadMeniuPreparate()
        {
            PreparateInMeniu.Clear();
            PreparateDisponibile.Clear();

            if (SelectedMeniu == null)
                return;

            try
            {
                // Load preparate already in meniu
                var preparateMeniu = await _preparatMeniuService.GetPreparatMeniuriAsync(SelectedMeniu.Id);
                var preparateIds = preparateMeniu.Select(pm => pm.PreparatId).ToList();

                foreach (var pm in preparateMeniu)
                {
                    if (pm.Preparat != null)
                        PreparateInMeniu.Add(pm.Preparat);
                }

                // Load available preparate (not in this meniu)
                foreach (var p in AllPreparate)
                {
                    if (!preparateIds.Contains(p.Id))
                        PreparateDisponibile.Add(p);
                }

                // Debug info
                System.Diagnostics.Debug.WriteLine($"Preparate în meniu: {PreparateInMeniu.Count}");
                System.Diagnostics.Debug.WriteLine($"Preparate disponibile: {PreparateDisponibile.Count}");
                System.Diagnostics.Debug.WriteLine($"Total preparate: {AllPreparate.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading meniu preparate: {ex.Message}");
            }
        }

        private async Task AddPreparatToMeniuAsync()
        {
            try
            {
                if (SelectedMeniu == null || SelectedPreparatToAdd == null || CantitatePortieMeniu <= 0)
                    return;

                await _preparatMeniuService.AddToMeniuAsync(
                    SelectedMeniu.Id,
                    SelectedPreparatToAdd.Id,
                    CantitatePortieMeniu);

                // Refresh lists
                LoadMeniuPreparate();

                // Reset inputs
                CantitatePortieMeniu = 0;
                SelectedPreparatToAdd = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding preparat to meniu: {ex.Message}");
            }
        }

        private async Task RemovePreparatFromMeniuAsync()
        {
            try
            {
                if (SelectedMeniu == null || SelectedPreparatToRemove == null)
                {
                    System.Windows.MessageBox.Show("Selectați un preparat din meniu pentru a-l elimina!",
                        "Selecție invalidă", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Removing preparat {SelectedPreparatToRemove.Denumire} from meniu {SelectedMeniu.Denumire}");

                // Confirmare ștergere
                var result = System.Windows.MessageBox.Show(
                    $"Doriți să eliminați preparatul '{SelectedPreparatToRemove.Denumire}' din meniul '{SelectedMeniu.Denumire}'?",
                    "Confirmare ștergere",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Question);

                if (result == System.Windows.MessageBoxResult.No)
                    return;

                await _spService.RemovePreparatFromMeniuAsync(SelectedMeniu.Id, SelectedPreparatToRemove.Id);
                System.Diagnostics.Debug.WriteLine("Preparat removed successfully");

                // Refresh lists
                LoadMeniuPreparate();
                SelectedPreparatToRemove = null;

                // Notificare
                System.Windows.MessageBox.Show("Preparatul a fost eliminat cu succes din meniu!",
                    "Succes", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error removing preparat from meniu: {ex.Message}");
                System.Windows.MessageBox.Show($"Eroare la eliminarea preparatului din meniu: {ex.Message}",
                    "Eroare", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }
}