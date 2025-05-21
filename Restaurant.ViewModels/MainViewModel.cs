// Restaurant.ViewModels/MainViewModel.cs
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.ViewModels.Common;
using System;

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
        public MeniuAdminViewModel MeniuAdminVM { get; } = new MeniuAdminViewModel();
        public StocRedusViewModel StocRedusVM { get; } = new StocRedusViewModel();
        public IstoricComenziViewModel IstoricComenziVM { get; }

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
                    OnPropertyChanged(nameof(IsGuest));
                    OnPropertyChanged(nameof(IsClient));
                    OnPropertyChanged(nameof(IsAngajat));
                    OnPropertyChanged(nameof(IsClientOrAngajat));
                }
            }
        }

        // Helpers for XAML bindings
        public bool IsGuest => CurrentRole == UserRole.Guest;
        public bool IsClient => CurrentRole == UserRole.Client;
        public bool IsAngajat => CurrentRole == UserRole.Angajat;
        public bool IsClientOrAngajat => CurrentRole == UserRole.Client || CurrentRole == UserRole.Angajat;

        // Constructor
        public MainViewModel()
        {
            // Inițializăm IstoricComenziViewModel
            IstoricComenziVM = new IstoricComenziViewModel();

            // Configurăm event handler pentru ComandaPlasata
            ComandaVM.ComandaPlasata += codUnic =>
            {
                // Reîncărcăm istoricul comenzilor după plasarea unei comenzi noi
                _ = IstoricComenziVM.LoadComenziAsync();
            };

            // Dacă ComandaVM nu are deja event-ul ComandaPlasata, adăugați-l
          
        }
    }
}