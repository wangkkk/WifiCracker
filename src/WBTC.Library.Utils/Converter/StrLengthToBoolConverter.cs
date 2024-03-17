using System;
using System.Globalization;
using System.Windows.Data;

namespace WBTC.Library.Utils.Converter
{
    public class StrLengthToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string v = value?.ToString();
            return !string.IsNullOrEmpty(v);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
