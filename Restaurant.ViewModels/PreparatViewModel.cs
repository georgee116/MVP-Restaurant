// Restaurant.ViewModels/PreparatViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using Microsoft.Win32; // Adăugat pentru OpenFileDialog
using System.IO;
using Restaurant.Domain.Entities;
using Restaurant.Services.Services;
using Restaurant.ViewModels.Common;

namespace Restaurant.ViewModels
{
    public class PreparatViewModel : BaseViewModel
    {
        // Servicii
        private readonly PreparatService _preparatService = new();
        private readonly CategorieService _categorieService = new();
        private readonly AlergenService _alergenService = new();
        private readonly PreparatAlergenService _preparatAlergenService = new();
        private readonly ImaginePreparatService _imagineService = new(); // Service pentru imagini

        // Colecții observabile
        public ObservableCollection<Preparat> Preparate { get; } = new();
        public ObservableCollection<Categorie> Categorii { get; } = new();
        public ObservableCollection<Alergen> AlergeniDisponibili { get; } = new();
        public ObservableCollection<Alergen> AlergeniSelectati { get; } = new();
        public ObservableCollection<ImaginePreparat> ImaginiPreparat { get; } = new();

        // Filtrare
        private string _filterText = string.Empty;
        public string FilterText
        {
            get => _filterText;
            set
            {
                _filterText = value;
                OnPropertyChanged();
                FilterPreparate();
            }
        }

        // Proprietate pentru noul preparat
        private Preparat _newPreparat = new Preparat
        {
            Denumire = "",
            Pret = 0,
            CantitatePortie = 0,
            CantitateTotala = 0,
            CategorieId = 0
        };

        public Preparat NewPreparat
        {
            get => _newPreparat;
            set
            {
                _newPreparat = value;
                OnPropertyChanged();
            }
        }

        // Proprietate pentru categoria selectată a noului preparat
        private Categorie _selectedNewPreparatCategorie;
        public Categorie SelectedNewPreparatCategorie
        {
            get => _selectedNewPreparatCategorie;
            set
            {
                _selectedNewPreparatCategorie = value;
                if (value != null)
                {
                    NewPreparat.CategorieId = value.Id;
                }
                OnPropertyChanged();
                ((RelayCommand)AddCommand).RaiseCanExecuteChanged();
            }
        }

        // Preparat selectat din listă
        private Preparat _selectedPreparat;
        public Preparat SelectedPreparat
        {
            get => _selectedPreparat;
            set
            {
                _selectedPreparat = value;
                OnPropertyChanged();
                ((RelayCommand)UpdateCommand).RaiseCanExecuteChanged();
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();

                if (value != null)
                {
                    LoadPreparatDetails();
                }
                else
                {
                    AlergeniSelectati.Clear();
                    ImaginiPreparat.Clear();
                }
            }
        }

        // Alergeni pentru adăugare/eliminare
        private Alergen _selectedAlergenToAdd;
        public Alergen SelectedAlergenToAdd
        {
            get => _selectedAlergenToAdd;
            set
            {
                _selectedAlergenToAdd = value;
                OnPropertyChanged();
                ((RelayCommand)AddAlergenCommand).RaiseCanExecuteChanged();
            }
        }

        private Alergen _selectedAlergenToRemove;
        public Alergen SelectedAlergenToRemove
        {
            get => _selectedAlergenToRemove;
            set
            {
                _selectedAlergenToRemove = value;
                OnPropertyChanged();
                ((RelayCommand)RemoveAlergenCommand).RaiseCanExecuteChanged();
            }
        }

        // Comenzi
        public ICommand LoadCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand AddAlergenCommand { get; }
        public ICommand RemoveAlergenCommand { get; }
        public ICommand AddImageCommand { get; }
        public ICommand RemoveImageCommand { get; }
        public ICommand UpdateStocCommand { get; }

        // Selector pentru imagine selectată pentru eliminare
        private ImaginePreparat _selectedImage;
        public ImaginePreparat SelectedImage
        {
            get => _selectedImage;
            set
            {
                _selectedImage = value;
                OnPropertyChanged();
                ((RelayCommand)RemoveImageCommand).RaiseCanExecuteChanged();
            }
        }

        public PreparatViewModel()
        {
            // Configurare comenzi
            LoadCommand = new RelayCommand(async _ => await LoadAsync());
            AddCommand = new RelayCommand(async _ => await AddAsync(), CanAdd);
            UpdateCommand = new RelayCommand(async _ => await UpdateAsync(), _ => SelectedPreparat != null);
            DeleteCommand = new RelayCommand(async _ => await DeleteAsync(), _ => SelectedPreparat != null);

            AddAlergenCommand = new RelayCommand(async _ => await AddAlergenAsync(), _ => SelectedAlergenToAdd != null && SelectedPreparat != null);
            RemoveAlergenCommand = new RelayCommand(async _ => await RemoveAlergenAsync(), _ => SelectedAlergenToRemove != null && SelectedPreparat != null);

            AddImageCommand = new RelayCommand(async _ => await AddImageAsync(), _ => SelectedPreparat != null);
            RemoveImageCommand = new RelayCommand(async _ => await RemoveImageAsync(), _ => SelectedImage != null && SelectedPreparat != null);

            UpdateStocCommand = new RelayCommand(async param => await UpdateStocAsync((float)param), _ => SelectedPreparat != null);

            // Inițializare
            _ = LoadAsync();
        }

        private bool CanAdd(object _)
        {
            return !string.IsNullOrWhiteSpace(NewPreparat.Denumire) &&
            SelectedNewPreparatCategorie != null;
        }

        private void FilterPreparate()
        {
            // Implementare logică de filtrare
            if (string.IsNullOrWhiteSpace(FilterText))
            {
                // Reîncarcă toate preparatele
                _ = LoadAsync();
                return;
            }

            // Altfel, filtrează lista existentă
            var filteredList = Preparate.Where(p =>
                p.Denumire.Contains(FilterText, StringComparison.OrdinalIgnoreCase)).ToList();

            Preparate.Clear();
            foreach (var p in filteredList)
            {
                Preparate.Add(p);
            }
        }

        public async Task LoadAsync()
        {
            // Încărcare preparate
            Preparate.Clear();
            var preparateList = await _preparatService.GetAllAsync();
            foreach (var p in preparateList)
            {
                Preparate.Add(p);
            }

            // Încărcare categorii
            Categorii.Clear();
            var categoriiList = await _categorieService.GetAllAsync();
            foreach (var c in categoriiList)
            {
                Categorii.Add(c);
            }

            // Încărcare alergeni disponibili
            AlergeniDisponibili.Clear();
            var alergeniList = await _alergenService.GetAllAlergeniAsync();
            foreach (var a in alergeniList)
            {
                AlergeniDisponibili.Add(a);
            }

            // Reset preparat nou
            NewPreparat = new Preparat
            {
                Denumire = "",
                Pret = 0,
                CantitatePortie = 0,
                CantitateTotala = 0,
                CategorieId = 0
            };
            SelectedNewPreparatCategorie = null;
        }

        private async Task AddAsync()
        {
            // Adaugă preparat nou
            await _preparatService.AddAsync(NewPreparat);
            Preparate.Add(NewPreparat);

            // Reset valorile
            NewPreparat = new Preparat
            {
                Denumire = "",
                Pret = 0,
                CantitatePortie = 0,
                CantitateTotala = 0,
                CategorieId = 0
            };
            SelectedNewPreparatCategorie = null;
        }

        private async Task UpdateAsync()
        {
            if (SelectedPreparat == null) return;
            await _preparatService.UpdateAsync(SelectedPreparat);
            await LoadAsync();
        }

        private async Task DeleteAsync()
        {
            if (SelectedPreparat == null) return;
            await _preparatService.DeleteAsync(SelectedPreparat.Id);
            Preparate.Remove(SelectedPreparat);
            SelectedPreparat = null;
        }

        private async void LoadPreparatDetails()
        {
            if (SelectedPreparat == null) return;

            // Încarcă alergenii preparatului
            AlergeniSelectati.Clear();
            var alergeni = await _preparatAlergenService.GetAlergeniForPreparatAsync(SelectedPreparat.Id);
            foreach (var a in alergeni)
            {
                AlergeniSelectati.Add(a);
            }

            // Încarcă imaginile preparatului
            ImaginiPreparat.Clear();
            var imagini = await _preparatService.GetImaginiPreparatAsync(SelectedPreparat.Id);
            foreach (var img in imagini)
            {
                ImaginiPreparat.Add(img);
            }
        }

        private async Task AddAlergenAsync()
        {
            if (SelectedPreparat == null || SelectedAlergenToAdd == null) return;

            // Verifică dacă preparatul are deja acest alergen
            if (AlergeniSelectati.Any(a => a.Id == SelectedAlergenToAdd.Id))
                return;

            await _preparatAlergenService.AddAlergenAsync(SelectedPreparat.Id, SelectedAlergenToAdd.Id);
            AlergeniSelectati.Add(SelectedAlergenToAdd);
        }

        private async Task RemoveAlergenAsync()
        {
            if (SelectedPreparat == null || SelectedAlergenToRemove == null) return;
            await _preparatAlergenService.RemoveAlergenAsync(SelectedPreparat.Id, SelectedAlergenToRemove.Id);
            AlergeniSelectati.Remove(SelectedAlergenToRemove);
        }

        private async Task AddImageAsync()
        {
            if (SelectedPreparat == null) return;

            // Implementează logica pentru selectarea și încărcarea imaginilor
            var dialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                // Implementare de bază - în producție va trebui adaptată
                string fileName = dialog.FileName;
                string destPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images",
                                              $"prep_{SelectedPreparat.Id}_{Path.GetFileName(fileName)}");

                // Asigură-te că directorul există
                Directory.CreateDirectory(Path.GetDirectoryName(destPath));

                // Copiază fișierul
                File.Copy(fileName, destPath, true);

                // Creează o nouă imagine în baza de date
                var imagine = new ImaginePreparat
                {
                    PreparatId = SelectedPreparat.Id,
                    PathImagine = destPath
                };

                // Adaugă imaginea
                var savedImage = await _imagineService.AddAsync(imagine);
                ImaginiPreparat.Add(savedImage);
            }
        }

        private async Task RemoveImageAsync()
        {
            if (SelectedPreparat == null || SelectedImage == null) return;

            // Șterge imaginea din baza de date
            await _imagineService.DeleteAsync(SelectedImage.Id);

            // Șterge fișierul fizic dacă există
            if (File.Exists(SelectedImage.PathImagine))
            {
                File.Delete(SelectedImage.PathImagine);
            }

            ImaginiPreparat.Remove(SelectedImage);
        }

        private async Task UpdateStocAsync(float cantitateNoua)
{
    if (SelectedPreparat == null) return;
    
    try
    {
        await _preparatService.UpdateStocAsync(SelectedPreparat.Id, cantitateNoua);
        SelectedPreparat.CantitateTotala = cantitateNoua;
        System.Windows.MessageBox.Show($"Stocul pentru {SelectedPreparat.Denumire} a fost actualizat la {cantitateNoua}g.",
            "Stoc actualizat", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
    }
    catch (Exception ex)
    {
        System.Windows.MessageBox.Show($"Eroare la actualizarea stocului: {ex.Message}",
            "Eroare", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
    }
}
    }
}