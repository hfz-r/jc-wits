using DataLayer;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace ESD.JC_FinishGoods.ViewModels
{
    public class AHUDetailsTransactionViewModel : BindableBase
    {
        public string ViewName
        {
            get { return "Transaction Details"; }
        }

        private ObservableCollection<AHUTransaction> _trnxCollection;
        public ObservableCollection<AHUTransaction> trnxCollection
        {
            get { return _trnxCollection; }
            set
            {
                SetProperty(ref _trnxCollection, value);
                RaisePropertyChanged("trnxCollection");
            }
        }

        public AHUDetailsTransactionViewModel()
        {
        }
    }
}
