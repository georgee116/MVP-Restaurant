using System.Collections.ObjectModel;
using System.Threading.Tasks;
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
        private Meniu? _selectedMeniu;
        public Meniu? SelectedMeniu
        {
            get => _selectedMeniu;
            set
            {
                if (_selectedMeniu != value)
                {
                    _selectedMeniu = value;
                    OnPropertyChanged();
                    LoadMeniuDetailsAsync();
                }
            }
        }

        // 3) detaliile meniului curent
        public ObservableCollection<MeniuDetailDto> Items { get; } = new();

        private MeniuTotalsDto? _totals;
        public MeniuTotalsDto? Totals
        {
            get => _totals;
            private set { _totals = value; OnPropertyChanged(); }
        }

        public MeniuViewModel()
        {
            // încărcați meniurile la pornire
            Task.Run(async () =>
            {
                var list = await _meniuService.GetAllAsync();
                foreach (var m in list)
                    AllMeniuri.Add(m);
            });
        }

        private async void LoadMeniuDetailsAsync()
        {
            Items.Clear();
            Totals = null;

            if (SelectedMeniu == null)
                return;

            // apel SP: ia lista de preparate + totals
            var result = await _spService.GetMeniuDetailsAsync(SelectedMeniu.Id);
            foreach (var dto in result.Items)
                Items.Add(dto);

            Totals = result.Totals;
        }
    }
}
