using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ESD.JC_Infrastructure
{
    public class UnitKGsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            string eun = (string)value;
            if (eun == "KG")
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
