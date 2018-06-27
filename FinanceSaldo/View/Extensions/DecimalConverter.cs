using System;
using System.Globalization;
using System.Windows.Data;

namespace FinanceSaldo.View.Extensions
{
    [ValueConversion(typeof(decimal), typeof(string))]
    public class DecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() ?? Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string data))
            {
                return value;
            }

            if (data.Equals(string.Empty))
            {
                return 0;
            }

            if (string.IsNullOrEmpty(data)) return Binding.DoNothing;

            //Hold the value if ending with .
            if (data.EndsWith(".") || data.Equals("-0"))
            {
                return Binding.DoNothing;
            }

            return decimal.TryParse(data, out var result) ? result : Binding.DoNothing;
        }
    } 
}
