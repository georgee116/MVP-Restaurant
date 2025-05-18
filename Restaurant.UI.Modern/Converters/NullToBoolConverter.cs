using System;
using System.Globalization;
using System.Windows.Data;

namespace Restaurant.UI.Modern.Converters
{
    public class NullToBoolConverter : IValueConverter
    {
        // Returnează true dacă valoarea nu e null
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
