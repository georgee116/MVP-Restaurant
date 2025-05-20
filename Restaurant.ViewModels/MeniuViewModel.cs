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

        // 1) toată colecția de meniuri
        public ObservableCollection<Meniu> AllMeniuri { get; } = new();

        // 2) meniu-ul selectat
        private Meniu _selectedMeniu;
        public Meniu SelectedMeniu
        {
            get => _selectedMeniu;
            set
            {
                if (_selectedMeniu != value)
                {
                    _selectedMeniu = value;
                    OnPropertyChanged();
                    // Încărcăm detaliile meniuului selectat
                    LoadMeniuDetailsAsync();
                }
            }
        }

        // 3) detaliile meniului curent
        public ObservableCollection<MeniuDetailDto> Items { get; } = new();

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

            try
            {
                // Apelăm SP pentru a obține lista de preparate și totaluri
                var result = await _spService.GetMeniuDetailsAsync(SelectedMeniu.Id);
                foreach (var dto in result.Items)
                    Items.Add(dto);

                Totals = result.Totals;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Eroare la încărcarea detaliilor meniului: {ex.Message}");
                Items.Clear();
                Totals = null;
            }
        }
    }
}