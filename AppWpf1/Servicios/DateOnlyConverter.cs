using System;
using System.Globalization;
using System.Windows.Data;

namespace AppWpf1.Servicios
{
    public class DateOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is DateTime dt ? dt.Date : null;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is DateTime dt ? dt.Date : DateTime.MinValue;
    }
}
