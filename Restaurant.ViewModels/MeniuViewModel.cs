// Restaurant.ViewModels/MeniuViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Entities;
using Restaurant.Services.Services;
using Restaurant.ViewModels.Common;

namespace Restaurant.ViewModels
{
    public class MeniuViewModel : BaseViewModel
    {
        private readonly MeniuService _meniuService = new();
        private readonly StoredProceduresService _spService = new();
        private readonly PreparatService _preparatService = new();

        // 1) toată colecția de meniuri
        public ObservableCollection<Meniu> AllMeniuri { get; } = new();

        // 2) meniu-ul selectat
        private Meniu _selectedMeniu;
        // În MeniuViewModel.cs
        public Meniu SelectedMeniu
        {
            get => _selectedMeniu;
            set
            {
                if (_selectedMeniu != value)
                {
                    _selectedMeniu = value;
                    // Pentru debugging
                    System.Diagnostics.Debug.WriteLine($"Meniu selectat: {value?.Denumire ?? "null"}, ID: {value?.Id ?? -1}");
                    OnPropertyChanged();
                    // Încărcăm detaliile meniuului selectat
                    LoadMeniuDetailsAsync();
                }
            }
        }

        // 3) clasa extinsă pentru itemurile meniului cu detalii despre disponibilitate
        public class MeniuItemViewModel : MeniuDetailDto
        {
            public bool Disponibil { get; set; }
            public string DisponibilText => Disponibil ? "Da" : "Nu";
        }

        // 4) detaliile meniului curent
        public ObservableCollection<MeniuItemViewModel> Items { get; } = new();

        private MeniuTotalsDto _totals;
        public MeniuTotalsDto Totals
        {
            get => _totals;
            private set
            {
                _totals = value;
                OnPropertyChanged();
            }
        }

        // 5) Proprietate pentru disponibilitatea meniului
        private bool _isMenuAvailable;
        public bool IsMenuAvailable
        {
            get => _isMenuAvailable;
            set
            {
                _isMenuAvailable = value;
                OnPropertyChanged();
            }
        }

        // Adăugăm o comandă pentru încărcarea meniurilor
        public ICommand LoadMeniuriCommand { get; }

        public MeniuViewModel()
        {
            // Adăugăm comanda
            LoadMeniuriCommand = new RelayCommand(async _ => await LoadMeniuriAsync());

            // Încărcăm meniurile la inițializare
            _ = LoadMeniuriAsync();
        }

        public async Task LoadMeniuriAsync()
        {
            try
            {
                AllMeniuri.Clear();
                var list = await _meniuService.GetAllAsync();
                foreach (var m in list)
                    AllMeniuri.Add(m);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Eroare la încărcarea meniurilor: {ex.Message}");
            }
        }

        private async void LoadMeniuDetailsAsync()
        {
            Items.Clear();
            Totals = null;

            if (SelectedMeniu == null)
                return;

            System.Diagnostics.Debug.WriteLine($"LoadMeniuDetailsAsync: Încărcare detalii pentru meniul {SelectedMeniu.Denumire} (ID: {SelectedMeniu.Id})");

            try
            {
                // Apelăm SP pentru a obține lista de preparate și totaluri
                var result = await _spService.GetMeniuDetailsAsync(SelectedMeniu.Id);

                System.Diagnostics.Debug.WriteLine($"Rezultat SP: {result.Items.Count()} preparate, Totals: {(result.Totals != null ? "există" : "null")}");

                foreach (var dto in result.Items)
                {
                    System.Diagnostics.Debug.WriteLine($"Preparat: {dto.Preparat}, GramajPortie: {dto.GramajPortie}, PretStandard: {dto.PretStandard}");

                    bool isDisponibil = true;
                    if (dto.PreparatId > 0)
                    {
                        var preparat = await _preparatService.GetByIdAsync(dto.PreparatId);
                        isDisponibil = preparat != null && preparat.CantitateTotala >= dto.GramajPortie;
                    }

                    Items.Add(new MeniuItemViewModel
                    {
                        MeniuId = dto.MeniuId,
                        Denumire = dto.Denumire,
                        Categorie = dto.Categorie,
                        PreparatId = dto.PreparatId,
                        Preparat = dto.Preparat,
                        GramajPortie = dto.GramajPortie,
                        PretStandard = dto.PretStandard,
                        Subtotal = dto.Subtotal,
                        Disponibil = isDisponibil
                    });
                }

                Totals = result.Totals;

                System.Diagnostics.Debug.WriteLine($"Items adăugate în colecție: {Items.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Eroare la încărcarea detaliilor meniului: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                Items.Clear();
                Totals = null;
            }
        }

        // Metodă pentru a verifica disponibilitatea unui meniu
        public async Task<bool> VerificaDisponibilitateMeniuAsync(int meniuId)
        {
            try
            {
                var result = await _spService.GetMeniuDetailsAsync(meniuId);

                foreach (var dto in result.Items)
                {
                    if (dto.PreparatId > 0)
                    {
                        var preparat = await _preparatService.GetByIdAsync(dto.PreparatId);
                        if (preparat == null || preparat.CantitateTotala < dto.GramajPortie)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Eroare la verificarea disponibilității meniului: {ex.Message}");
                return false;
            }
        }

        // Metodă pentru a obține cantitățile necesare pentru un meniu
        public async Task<Dictionary<int, float>> GetCantitatiNecesareMeniuAsync(int meniuId)
        {
            var cantitati = new Dictionary<int, float>();

            try
            {
                var result = await _spService.GetMeniuDetailsAsync(meniuId);

                foreach (var dto in result.Items)
                {
                    if (dto.PreparatId > 0)
                    {
                        cantitati[dto.PreparatId] = dto.GramajPortie;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Eroare la obținerea cantităților pentru meniu: {ex.Message}");
            }

            return cantitati;
        }
    }
}