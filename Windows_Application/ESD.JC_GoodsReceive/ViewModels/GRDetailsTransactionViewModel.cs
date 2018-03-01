using DataLayer;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace ESD.JC_GoodsReceive.ViewModels
{
    public class GRDetailsTransactionViewModel : BindableBase
    {
        public string ViewName
        {
            get { return "Transaction Details"; }
        }

        private ObservableCollection<GRTransaction> _trnxCollection;
        public ObservableCollection<GRTransaction> trnxCollection
        {
            get { return _trnxCollection; }
            set
            {
                SetProperty(ref _trnxCollection, value);
                RaisePropertyChanged("trnxCollection");
            }
        }

        public GRDetailsTransactionViewModel()
        {
        }
    }
}
