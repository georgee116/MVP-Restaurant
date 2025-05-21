// Restaurant.UI.Modern/Converters/GreaterZeroConverter.cs
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Restaurant.UI.Modern.Converters
{
    public class GreaterZeroConverter : IValueConverter
    {
        // Returnează true dacă valoarea e > 0
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isGreaterThanZero = false;

            if (value is int i)
                isGreaterThanZero = i > 0;
            else if (value is decimal d)
                isGreaterThanZero = d > 0;
            else if (value is float f) 
                isGreaterThanZero = f > 0;
            else if (value is double dbl)
                isGreaterThanZero = dbl > 0;
            else if (value is long l)
                isGreaterThanZero = l > 0;
            else if (value is short s)
                isGreaterThanZero = s > 0;
            else if (value is byte b)
                isGreaterThanZero = b > 0;
            else if (value != null && int.TryParse(value.ToString(), out int parsed))
                isGreaterThanZero = parsed > 0;

            // Verifică dacă trebuie să inversăm rezultatul
            if (parameter is string paramStr && paramStr == "Invert")
                isGreaterThanZero = !isGreaterThanZero;

            // Verifică dacă trebuie să convertim la Visibility
            if (parameter is string paramStr2 && paramStr2 == "Visibility")
                return isGreaterThanZero ? Visibility.Visible : Visibility.Collapsed;

            return isGreaterThanZero;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}