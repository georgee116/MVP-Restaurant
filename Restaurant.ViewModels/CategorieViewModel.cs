// Restaurant.ViewModels/CategorieViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Restaurant.Domain.Entities;
using Restaurant.Services.Services;
using Restaurant.ViewModels.Common;

namespace Restaurant.ViewModels
{
    public class CategorieViewModel : BaseViewModel
    {
        private readonly CategorieService _service = new();

        public ObservableCollection<Categorie> Categorii { get; } = new();

        private Categorie? _selected;
        public Categorie? SelectedCategorie
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
        public string NewCategorieNume
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

        public CategorieViewModel()
        {
            // 1) configure commands
            LoadCommand = new RelayCommand(async _ => await LoadAsync());
            AddCommand = new RelayCommand(async _ => await AddAsync(), _ => !string.IsNullOrWhiteSpace(NewCategorieNume));
            UpdateCommand = new RelayCommand(async _ => await UpdateAsync(), _ => SelectedCategorie != null);
            DeleteCommand = new RelayCommand(async _ => await DeleteAsync(), _ => SelectedCategorie != null);

            // 2) auto-load imediat
            _ = LoadAsync();
        }

        public async Task LoadAsync()
        {
            Categorii.Clear();
            var list = await _service.GetAllAsync();
            foreach (var c in list)
                Categorii.Add(c);
        }

        private async Task AddAsync()
        {
            var c = await _service.CreateAsync(NewCategorieNume);
            Categorii.Add(c);
            NewCategorieNume = string.Empty;
        }

        private async Task UpdateAsync()
        {
            if (SelectedCategorie is null) return;
            await _service.UpdateAsync(SelectedCategorie);
            await LoadAsync();
        }

        private async Task DeleteAsync()
        {
            if (SelectedCategorie is null) return;
            await _service.DeleteAsync(SelectedCategorie.Id);
            Categorii.Remove(SelectedCategorie);
            SelectedCategorie = null;
        }
    }
}