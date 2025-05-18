// Restaurant.ViewModels/Common/BaseViewModel.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Restaurant.ViewModels.Common
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
