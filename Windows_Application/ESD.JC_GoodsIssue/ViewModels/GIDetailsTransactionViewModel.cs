using DataLayer;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ESD.JC_GoodsIssue.ViewModels
{
    public class GIDetailsTransactionViewModel : BindableBase
    {
        public string ViewName
        {
            get { return "Transaction Details"; }
        }

        private ObservableCollection<GITransaction> _trnxCollection;
        public ObservableCollection<GITransaction> trnxCollection
        {
            get { return _trnxCollection; }
            set
            {
                SetProperty(ref _trnxCollection, value);
                RaisePropertyChanged("trnxCollection");
            }
        }

        public GIDetailsTransactionViewModel()
        {
        }
    }

    public class DynamicTemplateSelector2 : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate dt = null;
            FrameworkElement elem = container as FrameworkElement;

            foreach (var gi in item as ObservableCollection<GITransaction>)
            {
                if (gi.TransferType == "Production")
                    dt = elem.FindResource("Production") as DataTemplate;
                if (gi.TransferType == "Posting")
                    dt = elem.FindResource("Posting") as DataTemplate;
            }
            return dt;
        }
    }
}
