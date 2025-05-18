// Restaurant.ViewModels/MeniuViewModel.cs
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
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
        private readonly StoredProceduresService _spService = new();
        private readonly PreparatMeniuService _pmService = new();
        private readonly PreparatService _pService = new();

        public ObservableCollection<MeniuDetailDto> Items { get; } = new();
        public ObservableCollection<Preparat> AllPreparate { get; } = new();

        private int _selectedMeniuId;
        public int SelectedMeniuId
        {
            get => _selectedMeniuId;
            set { _selectedMeniuId = value; OnPropertyChanged(); ((RelayCommand)LoadMeniuCommand).RaiseCanExecuteChanged(); }
        }

        private Preparat? _selectedPreparat;
        public Preparat? SelectedPreparat
        {
            get => _selectedPreparat;
            set { _selectedPreparat = value; OnPropertyChanged(); ((RelayCommand)AddToMeniuCommand).RaiseCanExecuteChanged(); }
        }

        private float _cantPortie;
        public float CantPortie
        {
            get => _cantPortie;
            set { _cantPortie = value; OnPropertyChanged(); ((RelayCommand)AddToMeniuCommand).RaiseCanExecuteChanged(); }
        }

        public ICommand LoadMeniuCommand { get; }
        public ICommand AddToMeniuCommand { get; }
        public ICommand RemoveFromMeniuCommand { get; }

        public MeniuViewModel()
        {
            LoadMeniuCommand = new RelayCommand(async _ => await LoadMeniuAsync(), _ => SelectedMeniuId > 0);
            AddToMeniuCommand = new RelayCommand(async _ => await AddAsync(), _ => SelectedPreparat != null && CantPortie > 0);
            RemoveFromMeniuCommand = new RelayCommand(async param =>
            {
                var dto = (MeniuDetailDto)param!;
                await _pmService.RemoveFromMeniuAsync(dto.MeniuId, dto.PreparatId);
                await LoadMeniuAsync();
            }, _ => true);

            // preload listă preparate pentru dropdown
            Task.Run(async () =>
            {
                var prep = await _pService.GetAllAsync();
                foreach (var p in prep) AllPreparate.Add(p);
            });
        }

        private async Task LoadMeniuAsync()
        {
            Items.Clear();

            // 1) await returns a ValueTuple<IEnumerable<MeniuDetailDto>, MeniuTotalsDto>
            var result = await _spService.GetMeniuDetailsAsync(SelectedMeniuId);

            // 2) extract each part explicitly
            var list = result.Item1;      // your list of MeniuDetailDto
            var totals = result.Item2;      // your totals DTO

            foreach (var i in list)
                Items.Add(i);

            // now you can do:
            // TotalGramaj = totals.TotalGramaj;
            // TotalPret   = totals.TotalPret;
        }


        private async Task AddAsync()
        {
            await _pmService.AddToMeniuAsync(SelectedMeniuId, SelectedPreparat!.Id, CantPortie);
            await LoadMeniuAsync();
        }
    }
}
