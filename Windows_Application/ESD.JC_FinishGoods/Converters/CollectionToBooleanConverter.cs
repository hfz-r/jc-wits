using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using DataLayer;
using System.ComponentModel;
using System.Collections;

namespace ESD.JC_FinishGoods.Converters
{
    public class CollectionToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                ICollectionView cv = value as ICollectionView;
    
                if (parameter as Type == typeof(FCU))
                {
                    var fcus = cv.Cast<FCU>().ToList();
                    return Validate(fcus);
                }
                else if(parameter as Type == typeof(AHU))
                {
                    var acus = cv.Cast<AHU>().ToList();
                    return Validate(acus);
                }
            }
            return false;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private bool? Validate<T>(List<T> list)
        {
            if (list != null && list.Count() > 0)
            {
                var total = list.Count();
                int ischecked = 0;

                foreach (var item in list)
                    foreach (var prop in typeof(T).GetProperties())
                    {
                        if (prop.Name == "IsChecked")
                        {
                            var getval = prop.GetValue(item);
                            if ((bool)getval == true)
                                ischecked++;
                        }
                    }
                if (ischecked == total)
                    return true;
                if (ischecked > 0)
                    return null;
            }
            return false;
        }
    }
}