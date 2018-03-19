using System;
using System.Windows.Data;

namespace ESD.JC_Infrastructure
{
    public class StringToDecimalConverter : IValueConverter
    {
        public static string user_string = null;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (user_string != null)
            {
                return user_string;
            }

            decimal number = (decimal)value;
            return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}", number);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string s_value = value.ToString();
            decimal result = 0;

            if (!decimal.TryParse(s_value, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.CurrentCulture, out result))
                return null;

            user_string = s_value;

            return result;
        }
    }
}
