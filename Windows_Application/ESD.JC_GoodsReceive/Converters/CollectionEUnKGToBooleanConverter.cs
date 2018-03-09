using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.ComponentModel;
using ESD.JC_GoodsReceive.ModelsExt;

namespace ESD.JC_GoodsReceive.Converters
{
    public class CollectionEUnKGToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                ICollectionView collection = value as ICollectionView;

                List<EunKGExt> KGs = collection.SourceCollection.Cast<EunKGExt>().ToList();
                if (KGs != null && KGs.Count() > 0)
                {
                    int TotalKGs = KGs.Count();
                    int selectedKGs = KGs.Where(x => x.IsChecked).Count();

                    if (selectedKGs == TotalKGs)
                        return true;

                    if (selectedKGs > 0)
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
