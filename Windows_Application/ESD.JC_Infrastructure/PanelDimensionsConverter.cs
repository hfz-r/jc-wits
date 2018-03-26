using System;
using System.Windows.Data;

namespace ESD.JC_Infrastructure
{
    public class PanelWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int width = (int)value;
            int retVal = width * -1;
            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PanelDimensionsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double? totalWidth = 0;

            //Combine all the values passed to give a total width
            foreach (object o in values)
            {
                int current;
                bool parsed = int.TryParse(o.ToString(), out current);
                if (parsed)
                {
                    totalWidth += current;
                }
            }

            //ensure negative value for scolling left
            totalWidth *= -1;
            return totalWidth;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
