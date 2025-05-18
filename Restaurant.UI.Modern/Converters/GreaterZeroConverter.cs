using System;
using System.Globalization;
using System.Windows.Data;

namespace Restaurant.UI.Modern.Converters
{
    public class GreaterZeroConverter : IValueConverter
    {
        // Returnează true dacă valoarea e int > 0
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int i) return i > 0;
            if (value is null) return false;
            if (int.TryParse(value.ToString(), out var v)) return v > 0;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
