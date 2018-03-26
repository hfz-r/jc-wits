using System;
using System.Windows;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ESD.JC_Infrastructure
{
    public class OKcolumnColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] != null)
            {
                bool val = (bool)values[0];
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
                if (values[1] != null && ((string)values[1]).Contains("All"))
                {
                    LinearGradientBrush gradientBrush = new LinearGradientBrush(Color.FromRgb(0, 255, 0), Color.FromRgb(255, 0, 0), new Point(0.5, 0), new Point(0.5, 1));
                    return gradientBrush;
                }
                else
                    return Brushes.Orange;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
