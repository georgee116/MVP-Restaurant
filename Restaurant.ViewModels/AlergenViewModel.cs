// Restaurant.ViewModels/AlergenViewModel.cs
using System;
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

        private Alergen _selected;
        public Alergen SelectedAlergen
        {
            get => _selected;
            set
            {
                _selected = value;
                OnPropertyChanged();
                ((RelayCommand)UpdateCommand).RaiseCanExecuteChanged();
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
            }
        }

        private string _newNume = string.Empty;
        public string NewAlergenNume
        {
            get => _newNume;
            set
            {
                _newNume = value;
                OnPropertyChanged();
                ((RelayCommand)AddCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand LoadCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }

        public AlergenViewModel()
        {
            // 1) configurăm comenzile
            LoadCommand = new RelayCommand(async _ => await LoadAsync());
            AddCommand = new RelayCommand(async _ => await AddAsync(), _ => !string.IsNullOrWhiteSpace(NewAlergenNume));
            UpdateCommand = new RelayCommand(async _ => await UpdateAsync(), _ => SelectedAlergen != null);
            DeleteCommand = new RelayCommand(async _ => await DeleteAsync(), _ => SelectedAlergen != null);

            // 2) încărcăm inițial datele
            _ = LoadAsync();
        }

        public async Task LoadAsync()
        {
            try
            {
                Alergeni.Clear();
                var list = await _service.GetAllAlergeniAsync();
                foreach (var a in list)
                    Alergeni.Add(a);
            }
            catch (Exception ex)
            {
                // În producție, ar trebui să gestionați erorile corespunzător
                System.Diagnostics.Debug.WriteLine($"Eroare la încărcarea alergenilor: {ex.Message}");
            }
        }

        private async Task AddAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NewAlergenNume)) return;

                var a = await _service.CreateAlergenAsync(NewAlergenNume);
                Alergeni.Add(a);
                NewAlergenNume = string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Eroare la adăugarea alergenului: {ex.Message}");
            }
        }

        private async Task UpdateAsync()
        {
            try
            {
                if (SelectedAlergen == null) return;
                if (string.IsNullOrWhiteSpace(SelectedAlergen.Nume)) return;

                await _service.UpdateAlergenAsync(SelectedAlergen);

                // Reîncărcăm lista pentru a reflecta modificările
                await LoadAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Eroare la actualizarea alergenului: {ex.Message}");
            }
        }

        private async Task DeleteAsync()
        {
            try
            {
                if (SelectedAlergen is null) return;

                await _service.DeleteAlergenAsync(SelectedAlergen.Id);
                Alergeni.Remove(SelectedAlergen);
                SelectedAlergen = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Eroare la ștergerea alergenului: {ex.Message}");
            }
        }
    }
}