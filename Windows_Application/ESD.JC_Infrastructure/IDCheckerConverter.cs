using System;
using System.Globalization;
using System.Windows.Data;

namespace ESD.JC_Infrastructure
{
    public class IDCheckerConverter : IValueConverter
    {
        public long Cutoff { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((long)value) != Cutoff;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
