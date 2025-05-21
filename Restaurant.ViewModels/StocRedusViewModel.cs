// În Restaurant.ViewModels/StocRedusViewModel.cs
using Restaurant.Domain;
using Restaurant.Services.Services;
using Restaurant.ViewModels.Common;
using System.Collections.ObjectModel;
using System.Windows.Input;

public class StocRedusViewModel : BaseViewModel
{
    private readonly PreparatService _preparatService = new();
    private float _limitaStoc = 1000; // Valoarea implicită din AppSettings

    public ObservableCollection<PreparatStocRedusModel> PreparateStocRedus { get; } = new();

    public float LimitaStoc
    {
        get => _limitaStoc;
        set
        {
            _limitaStoc = value;
            OnPropertyChanged();
            _ = LoadAsync();
        }
    }

    public ICommand LoadCommand { get; }

    public StocRedusViewModel()
    {
        LoadCommand = new RelayCommand(async _ => await LoadAsync());

        // Încarcă limita din configurare
        LimitaStoc = AppSettings.Instance.LowStockThreshold;

        // Încarcă datele inițial
        _ = LoadAsync();
    }

    public async Task LoadAsync()
    {
        try
        {
            PreparateStocRedus.Clear();

            var preparate = await _preparatService.GetAllAsync();
            var stocRedus = preparate.Where(p => p.CantitateTotala <= LimitaStoc)
                                     .OrderBy(p => p.CantitateTotala)
                                     .ToList();

            foreach (var p in stocRedus)
            {
                PreparateStocRedus.Add(new PreparatStocRedusModel
                {
                    Id = p.Id,
                    Denumire = p.Denumire,
                    CantitateTotala = p.CantitateTotala,
                    CantitatePortie = p.CantitatePortie,
                    PortiiDisponibile = (int)Math.Floor(p.CantitateTotala / p.CantitatePortie)
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Eroare la încărcarea preparatelor cu stoc redus: {ex.Message}");
        }
    }

    public class PreparatStocRedusModel
    {
        public int Id { get; set; }
        public string Denumire { get; set; }
        public float CantitateTotala { get; set; }
        public float CantitatePortie { get; set; }
        public int PortiiDisponibile { get; set; }
    }
}