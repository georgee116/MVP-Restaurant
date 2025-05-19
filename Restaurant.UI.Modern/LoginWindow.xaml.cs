// Restaurant.UI.Modern/LoginWindow.xaml.cs
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.ViewModels;

namespace Restaurant.UI.Modern
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            if (DataContext is LoginViewModel vm)
            {
                vm.LoginSucceeded += OnLoginSucceeded;
                vm.RegistrationRequested += () =>
                {
                    var rw = new RegistrationWindow();
                    rw.Owner = this;
                    rw.ShowDialog();
                };
            }
        }

        private void OnLoginSucceeded(Utilizator? user, UserRole role)
        {
            var main = new MainWindow();
            if (main.DataContext is MainViewModel mvm)
            {
                mvm.CurrentUser = user;
                mvm.CurrentRole = role;
            }
            main.Show();
            this.Close();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
                vm.Password = ((PasswordBox)sender).Password;
        }
    }
}
