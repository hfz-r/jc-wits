using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ESD.JC_GoodsReceive.Converters
{
    public class QuantityFormattingConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(x => x == DependencyProperty.UnsetValue))
                return null;

            object qty;
            if (values[1] == null)
                qty = 0;
            else
            {
                if ((string) values[0] == "KG")
                    qty = string.Format("{0:N2}", (decimal) values[1]);
                else if ((string) values[0] == "M2" || (string) values[0] == "M3")
                    qty = string.Format("{0:N3}", (decimal) values[1]);
                else
                    qty = (decimal) values[1];
            }

            return string.Format("{0:G29}", qty);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
