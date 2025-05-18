using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.Services.Services;
using Restaurant.ViewModels.Common;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System;
using System.Threading.Tasks;



namespace Restaurant.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public bool IsLoginPanelVisible { get; set; }
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string ErrorMessage { get; set; } = "";

        private UserRole _role;

        public event Action<Utilizator?, UserRole>? LoginSucceeded;

        public ICommand ContinueAsGuestCommand { get; }
        public ICommand ShowClientLoginCommand { get; }
        public ICommand ShowEmployeeLoginCommand { get; }
        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            ContinueAsGuestCommand = new RelayCommand(_ => OnLoginSucceeded(null, UserRole.Guest));
            ShowClientLoginCommand = new RelayCommand(_ => ShowLogin(UserRole.Client));
            ShowEmployeeLoginCommand = new RelayCommand(_ => ShowLogin(UserRole.Angajat));
            LoginCommand = new RelayCommand(async _ => await DoLogin(), _ => CanLogin);
        }

        private void ShowLogin(UserRole role)
        {
            _role = role;
            ErrorMessage = "";
            IsLoginPanelVisible = true;
            OnPropertyChanged(nameof(IsLoginPanelVisible));
        }

        private async Task DoLogin()
        {
            var svc = new UtilizatorService();
            var user = await svc.AuthenticateAsync(Email, Password, _role);
            if (user != null)
                OnLoginSucceeded(user, _role);
            else
            {
                ErrorMessage = "Date de autentificare incorecte.";
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        // metoda care ridică evenimentul
        protected void OnLoginSucceeded(Utilizator? user, UserRole role)
            => LoginSucceeded?.Invoke(user, role);

        public bool CanLogin
            => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);
    }
}

