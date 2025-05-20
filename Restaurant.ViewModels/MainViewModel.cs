using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.ViewModels.Common;

namespace Restaurant.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        // Sub‐viewmodels
        public AlergenViewModel AlergenVM { get; } = new AlergenViewModel();
        public MeniuViewModel MeniuVM { get; } = new MeniuViewModel();
        public ComandaViewModel ComandaVM { get; } = new ComandaViewModel();
        public CategorieViewModel CategorieVM { get; } = new CategorieViewModel();
        public PreparatViewModel PreparatVM { get; } = new PreparatViewModel();

        // The logged‐in user (null if Guest)
        private Utilizator? _currentUser;
        public Utilizator? CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }

        // The current role
        private UserRole _currentRole = UserRole.Guest;
        public UserRole CurrentRole
        {
            get => _currentRole;
            set
            {
                if (_currentRole != value)
                {
                    _currentRole = value;
                    OnPropertyChanged();
                    // notify each boolean too:
                    OnPropertyChanged(nameof(IsGuest));
                    OnPropertyChanged(nameof(IsClient));
                    OnPropertyChanged(nameof(IsAngajat));
                }
            }
        }

        // Helpers for XAML bindings
        public bool IsGuest => CurrentRole == UserRole.Guest;
        public bool IsClient => CurrentRole == UserRole.Client;
        public bool IsAngajat => CurrentRole == UserRole.Angajat;
    }
}
