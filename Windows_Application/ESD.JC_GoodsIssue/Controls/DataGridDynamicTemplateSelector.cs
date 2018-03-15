using DataLayer;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ESD.JC_GoodsIssue.Controls
{
    public class DataGridDynamicTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate dt = null;
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null)
            {
                var gi = item as GITransaction;
                if (gi.TransferType == "Production")
                    dt = element.FindResource("Production") as DataTemplate;
                if (gi.TransferType == "Posting")
                    dt = element.FindResource("Posting") as DataTemplate;
            }
            return dt;
        }
    }
}
