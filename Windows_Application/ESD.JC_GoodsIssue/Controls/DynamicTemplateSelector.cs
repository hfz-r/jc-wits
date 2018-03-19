using DataLayer;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ESD.JC_GoodsIssue.Controls
{
    public class DynamicTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate dt = null;
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null)
            {
                var gi = item as ObservableCollection<GITransaction>;

                if (gi.First().TransferType == "TRANSFER_PROD")
                    dt = element.FindResource("Production") as DataTemplate;
                if (gi.First().TransferType == "TRANSFER_POST")
                    dt = element.FindResource("Posting") as DataTemplate;
            }
            return dt;
        }
    }
}
