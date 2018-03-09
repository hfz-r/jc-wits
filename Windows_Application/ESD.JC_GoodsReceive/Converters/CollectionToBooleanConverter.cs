using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using DataLayer;
using System.ComponentModel;

namespace ESD.JC_GoodsReceive.Converters
{
    public class CollectionToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                ICollectionView GoodReceives = value as ICollectionView;

                List<GoodsReceive> GRs = GoodReceives.Cast<GoodsReceive>().ToList();
                if (GRs != null && GRs.Count() > 0)
                {
                    int TotalGRs = GRs.Count();
                    int selectedGRs = GRs.Where(x => x.IsChecked).Count();

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
