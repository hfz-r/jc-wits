using System;
using System.Globalization;
using System.Windows.Data;

namespace ESD.JC_Infrastructure
{
    public class StringToDecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal retVal;
            return decimal.TryParse(value.ToString(), out retVal) ? retVal : 0;
        }
    }
}
