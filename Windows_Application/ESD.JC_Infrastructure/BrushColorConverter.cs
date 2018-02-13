using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ESD.JC_Infrastructure
{
    public class BrushColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                bool val = (bool)value;
                switch (val)
                {
                    case true:
                        return Brushes.Green;
                    case false:
                        return Brushes.Red;
                    default:
                        return DependencyProperty.UnsetValue;
                }
            }
            else
            {
                return Brushes.Orange;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
