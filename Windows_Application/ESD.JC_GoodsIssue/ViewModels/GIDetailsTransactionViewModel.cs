using DataLayer;
using Prism.Mvvm;
using System.Collections.ObjectModel;

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
}
