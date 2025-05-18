// Restaurant.ViewModels/AlergenViewModel.cs
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Restaurant.Domain.Entities;
using Restaurant.Services.Services;
using Restaurant.ViewModels.Common;

namespace Restaurant.ViewModels
{
    public class AlergenViewModel : BaseViewModel
    {
        private readonly AlergenService _service = new();

        public ObservableCollection<Alergen> Alergeni { get; } = new();

        private Alergen? _selected;
        public Alergen? SelectedAlergen
        {
            get => _selected;
            set { _selected = value; OnPropertyChanged(); }
        }

        private string _newNume = string.Empty;
        public string NewAlergenNume
        {
            get => _newNume;
            set { _newNume = value; OnPropertyChanged(); ((RelayCommand)AddCommand).RaiseCanExecuteChanged(); }
        }

        public ICommand LoadCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }

        public AlergenViewModel()
        {
            LoadCommand = new RelayCommand(async _ => await LoadAsync());
            AddCommand = new RelayCommand(async _ => await AddAsync(), _ => !string.IsNullOrWhiteSpace(NewAlergenNume));
            UpdateCommand = new RelayCommand(async _ => await UpdateAsync(), _ => SelectedAlergen != null);
            DeleteCommand = new RelayCommand(async _ => await DeleteAsync(), _ => SelectedAlergen != null);
        }

        public async Task LoadAsync()
        {
            Alergeni.Clear();
            var list = await _service.GetAllAlergeniAsync();
            foreach (var a in list)
                Alergeni.Add(a);
        }

        private async Task AddAsync()
        {
            var a = await _service.CreateAlergenAsync(NewAlergenNume);
            Alergeni.Add(a);
            NewAlergenNume = string.Empty;
        }

        private async Task UpdateAsync()
        {
            if (SelectedAlergen is null) return;
            await _service.UpdateAlergenAsync(SelectedAlergen);
            await LoadAsync();
        }

        private async Task DeleteAsync()
        {
            if (SelectedAlergen is null) return;
            await _service.DeleteAlergenAsync(SelectedAlergen.Id);
            Alergeni.Remove(SelectedAlergen);
            SelectedAlergen = null;
        }
    }
}
