// Restaurant.UI.Modern/Converters/NullToBoolConverter.cs
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Restaurant.UI.Modern.Converters
{
    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isNotNull = value != null;

            // Verifică dacă trebuie să inversăm rezultatul
            if (parameter is string paramStr && paramStr == "Invert")
            {
                isNotNull = !isNotNull;
            }

            // Verifică dacă trebuie să convertim la Visibility 
            if (parameter is string paramStr2 && paramStr2 == "Visibility")
            {
                return isNotNull ? Visibility.Visible : Visibility.Collapsed;
            }

            return isNotNull;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}