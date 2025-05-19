// Restaurant.ViewModels/RegistrationViewModel.cs
using System.Threading.Tasks;
using System.Windows.Input;
using Restaurant.Domain.Entities;
using Restaurant.Services.Services;
using Restaurant.ViewModels.Common;

namespace Restaurant.ViewModels
{
    public class RegistrationViewModel : BaseViewModel
    {
        private string _nume = "";
        private string _prenume = "";
        private string _email = "";
        private string _telefon = "";
        private string _adresaLivrare = "";
        private string _password = "";
        private string _confirmPassword = "";
        private string _errorMessage = "";

        public string Nume
        {
            get => _nume;
            set { _nume = value; OnPropertyChanged(); RaiseCanExec(); }
        }
        public string Prenume
        {
            get => _prenume;
            set { _prenume = value; OnPropertyChanged(); RaiseCanExec(); }
        }
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); RaiseCanExec(); }
        }
        public string Telefon
        {
            get => _telefon;
            set { _telefon = value; OnPropertyChanged(); RaiseCanExec(); }
        }
        public string AdresaLivrare
        {
            get => _adresaLivrare;
            set { _adresaLivrare = value; OnPropertyChanged(); RaiseCanExec(); }
        }
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); RaiseCanExec(); }
        }
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; OnPropertyChanged(); RaiseCanExec(); }
        }
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public event Action? RegistrationSucceeded;
        public event Action? RegistrationCancelled;

        public ICommand RegisterCommand { get; }
        public ICommand CancelCommand { get; }

        public RegistrationViewModel()
        {
            var svc = new UtilizatorService();

            RegisterCommand = new RelayCommand(async _ =>
            {
                ErrorMessage = "";
                if (Password != ConfirmPassword)
                {
                    ErrorMessage = "Parolele nu coincid.";
                    OnPropertyChanged(nameof(ErrorMessage));
                    return;
                }
                var u = new Utilizator
                {
                    Nume = Nume,
                    Prenume = Prenume,
                    Email = Email,
                    Telefon = Telefon,
                    AdresaLivrare = AdresaLivrare,
                    Parola = Password
                };
                await svc.RegisterAsync(u);
                // 2) ridică evenimentul de succes
                RegistrationSucceeded?.Invoke();
            }, _ => CanRegister);

            CancelCommand = new RelayCommand(_ =>
            {
                // 3) ridică evenimentul de anulare
                RegistrationCancelled?.Invoke();
            });
        }

        public bool CanRegister =>
            !string.IsNullOrWhiteSpace(Nume) &&
            !string.IsNullOrWhiteSpace(Prenume) &&
            !string.IsNullOrWhiteSpace(Email) &&
            !string.IsNullOrWhiteSpace(Password) &&
            !string.IsNullOrWhiteSpace(ConfirmPassword) &&
            Password == ConfirmPassword;

        private void RaiseCanExec()
            => ((RelayCommand)RegisterCommand).RaiseCanExecuteChanged();
    }
}
