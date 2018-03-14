using DataLayer;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace ESD.JC_FinishGoods.ViewModels
{
    public class FCUDetailsTransactionViewModel : BindableBase
    {
        public string ViewName
        {
            get { return "Transaction Details"; }
        }

        private ObservableCollection<FCUTransaction> _trnxCollection;
        public ObservableCollection<FCUTransaction> trnxCollection
        {
            get { return _trnxCollection; }
            set
            {
                SetProperty(ref _trnxCollection, value);
                RaisePropertyChanged("trnxCollection");
            }
        }

        public FCUDetailsTransactionViewModel()
        {
        }
    }
}
