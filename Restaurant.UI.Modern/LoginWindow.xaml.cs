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
            // te abonezi la eveniment
            if (DataContext is LoginViewModel vm)
                vm.LoginSucceeded += OnLoginSucceeded;
        }

        // când LoginSucceeded e invocat de VM, deschizi MainWindow
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
