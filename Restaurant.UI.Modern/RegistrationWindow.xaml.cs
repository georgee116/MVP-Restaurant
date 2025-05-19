using System.Windows;
using System.Windows.Controls;

namespace Restaurant.UI.Modern
{
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow() => InitializeComponent();

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is Restaurant.ViewModels.RegistrationViewModel vm)
                vm.Password = ((PasswordBox)sender).Password;
        }

        private void PasswordBox_ConfirmPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is Restaurant.ViewModels.RegistrationViewModel vm)
                vm.ConfirmPassword = ((PasswordBox)sender).Password;
        }
    }
}
