// Restaurant.ViewModels/LoginViewModel.cs
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.Services.Services;
using Restaurant.ViewModels.Common;

namespace Restaurant.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        // 1) State for showing either the initial selection or the login panel
        private bool _isLoginPanelVisible;
        public bool IsLoginPanelVisible
        {
            get => _isLoginPanelVisible;
            set
            {
                if (_isLoginPanelVisible != value)
                {
                    _isLoginPanelVisible = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsSelectionVisible));
                }
            }
        }
        public bool IsSelectionVisible => !IsLoginPanelVisible;

        // 2) Which role we’re currently authenticating
        private string _loginRoleName = "";
        public string LoginRoleName
        {
            get => _loginRoleName;
            set { _loginRoleName = value; OnPropertyChanged(); }
        }

        // 3) The credentials & error message
        private string _email = "";
        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged();
                    ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                    ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private string _errorMessage = "";
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        // 4) The role enum we’ll pass to the service
        private UserRole _role;

        // 5) Events to notify the View (window) what to do
        public event Action<Utilizator?, UserRole>? LoginSucceeded;
        public event Action? RegistrationRequested;

        // 6) Commands bound from XAML
        public ICommand ContinueAsGuestCommand { get; }
        public ICommand ShowClientLoginCommand { get; }
        public ICommand ShowEmployeeLoginCommand { get; }
        public ICommand LoginCommand { get; }
        public ICommand ShowRegistrationCommand { get; }
        public ICommand CancelLoginCommand { get; }

        public LoginViewModel()
        {
            ContinueAsGuestCommand = new RelayCommand(_ => OnLoginSucceeded(null, UserRole.Guest));
            ShowClientLoginCommand = new RelayCommand(_ => ShowLogin(UserRole.Client));
            ShowEmployeeLoginCommand = new RelayCommand(_ => ShowLogin(UserRole.Angajat));
            LoginCommand = new RelayCommand(async _ => await DoLogin(), _ => CanLogin);
            ShowRegistrationCommand = new RelayCommand(_ => OnRegistrationRequested());
            CancelLoginCommand = new RelayCommand(_ => CancelLogin());
        }

        // Called when you press “Client” or “Angajat”
        private void ShowLogin(UserRole role)
        {
            _role = role;
            LoginRoleName = role == UserRole.Client
                ? "Autentificare Client"
                : "Autentificare Angajat";
            ErrorMessage = "";
            IsLoginPanelVisible = true;
        }

        // The async call into your UtilizatorService
        private async Task DoLogin()
        {
            var svc = new UtilizatorService();
            var user = await svc.AuthenticateAsync(Email, Password, _role);
            if (user != null)
                OnLoginSucceeded(user, _role);
            else
                ErrorMessage = "Date de autentificare incorecte.";
        }

        // Raise the LoginSucceeded event
        protected void OnLoginSucceeded(Utilizator? user, UserRole role)
            => LoginSucceeded?.Invoke(user, role);

        // Raise the RegistrationRequested event
        protected void OnRegistrationRequested()
            => RegistrationRequested?.Invoke();

        // CanExecute for the Login button
        public bool CanLogin
            => !string.IsNullOrWhiteSpace(Email)
            && !string.IsNullOrWhiteSpace(Password);

        private void CancelLogin()
        {
            // ascunde panoul de login, arată iar selecția
            IsLoginPanelVisible = false;
            // opțional, golește câmpurile
            Email = "";
            Password = "";
            ErrorMessage = "";
            OnPropertyChanged(nameof(Email));
            OnPropertyChanged(nameof(Password));
            OnPropertyChanged(nameof(ErrorMessage));
        }

    }
}
