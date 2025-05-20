// Restaurant.UI.Modern/Converters/BoolToVisConverter.cs
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Restaurant.UI.Modern.Converters
{
    public class BoolToVisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                if (parameter is string paramStr && paramStr == "Invert")
                {
                    // Inversăm valoarea dacă parametrul este "Invert"
                    boolValue = !boolValue;
                }

                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility vis)
            {
                bool result = (vis == Visibility.Visible);

                if (parameter is string paramStr && paramStr == "Invert")
                {
                    // Inversăm valoarea la convertirea înapoi
                    result = !result;
                }

                return result;
            }

            return false;
        }
    }
}