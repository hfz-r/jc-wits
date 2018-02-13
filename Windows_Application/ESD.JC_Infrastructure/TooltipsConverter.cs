using System;
using System.Globalization;
using System.Windows.Data;

namespace ESD.JC_Infrastructure
{
    public class TooltipsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            decimal qtyRecieved;
            if (values[0] == null)
                qtyRecieved = 0;
            else
                qtyRecieved = (decimal)values[0];

            decimal qty;
            if (values[1] == null)
                qty = 0;
            else
                qty = (decimal)values[1];

            return string.Format("{0}/{1}", qtyRecieved, qty);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
