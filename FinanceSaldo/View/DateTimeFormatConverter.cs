using System;
using System.Data.SqlTypes;
using System.Globalization;
using System.Windows.Data;

namespace FinanceSaldo.View
{
    public class DateTimeFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = (DateTime)value;

            if (date < SqlDateTime.MinValue.Value)
            {
                date = SqlDateTime.MinValue.Value;
            }
            else if (date > SqlDateTime.MaxValue.Value)
            {
                date = SqlDateTime.MaxValue.Value;
            }

            if (date == SqlDateTime.MinValue) return DateTime.Now.Date;

            return date;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = (DateTime)value;

            if (date < SqlDateTime.MinValue.Value)
            {
                date = SqlDateTime.MinValue.Value;
            }
            else if (date > SqlDateTime.MaxValue.Value)
            {
                date = SqlDateTime.MaxValue.Value;
            }

            return date;
        }
    }
}
