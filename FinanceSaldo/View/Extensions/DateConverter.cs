using System;
using System.Globalization;
using System.Windows.Data;

namespace FinanceSaldo.View.Extensions
{
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dt = (DateTime)value;
            return dt.ToString("dd/MM/yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] formats = {"ddMMyyyy", "ddMMyy"};

            string str = (string) value;

            if (DateTime.TryParseExact(str, formats, culture, DateTimeStyles.None, out DateTime dateValue))
                return dateValue;
            else
                return 0;
        }
    }
}
