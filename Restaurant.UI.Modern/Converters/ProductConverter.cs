// Restaurant.UI.Modern/Converters/ProductConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;

namespace Restaurant.UI.Modern.Converters
{
    public class ProductConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Verificăm dacă avem cele două valori necesare: preț și cantitate
            if (values.Length < 2 || values[0] == null || values[1] == null)
                return 0;

            // Încercăm conversia: primul parametru este prețul, al doilea este cantitatea
            decimal pret = 0;
            int cantitate = 0;

            // Convertim prețul
            if (values[0] is decimal d)
                pret = d;
            else if (decimal.TryParse(values[0].ToString(), out decimal parsedDecimal)) 
                pret = parsedDecimal;
            else
                return 0;

            // Convertim cantitatea
            if (values[1] is int i)
                cantitate = i;
            else if (int.TryParse(values[1].ToString(), out int parsedInt))
                cantitate = parsedInt;
            else
                return 0;

            // Returnam produsul preț x cantitate
            return pret * cantitate;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}