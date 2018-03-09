using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using DataLayer;
using System.ComponentModel;

namespace ESD.JC_FinishGoods.Converters
{
    public class CollectionToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                ICollectionView FGs = value as ICollectionView;

                List<FCU> FCUs = FGs.Cast<FCU>().ToList();
                if (FCUs != null && FCUs.Count() > 0)
                {
                    int TotalGRs = FCUs.Count();
                    int selectedGRs = FCUs.Where(x => x.IsChecked).Count();

                    if (selectedGRs == TotalGRs)
                        return true;

                    if (selectedGRs > 0)
                        return null;
                }
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}