using DataLayer;
using ESD.JC_FinishGoods.Services;
using Prism.Mvvm;
using Prism.Regions;
using Microsoft.Practices.Unity;
using System.Linq;
using System.Collections.ObjectModel;

namespace ESD.JC_FinishGoods.ViewModels
{
    public class FCUDetailsSummaryViewModel : BindableBase
    {
        public string ViewName
        {
            get { return "Transaction Summary"; }
        }

        private ObservableCollection<FCUTransaction> _summryCollection;
        public ObservableCollection<FCUTransaction> summryCollection
        {
            get { return _summryCollection; }
            set
            {
                SetProperty(ref _summryCollection, value);
                if (summryCollection.Count() > 0)
                {
                    var fcuObj = summryCollection.Select(x => x.FCU).FirstOrDefault();
                    RemainingQty = fcuObj.Qty - fcuObj.QtyReceived.GetValueOrDefault();

                    CountCountries = summryCollection.GroupBy(c => c.Country.ID).Count();
                }

                RaisePropertyChanged("RemainingQty");
                RaisePropertyChanged("CountCountries");
            }
        }

        private decimal _RemainingQty = 0;
        public decimal RemainingQty
        {
            get { return _RemainingQty; }
            set { SetProperty(ref _RemainingQty, value); }
        }

        private int _CountCountries;
        public int CountCountries
        {
            get { return _CountCountries; }
            set { SetProperty(ref _CountCountries, value); }
        }

        private IUnityContainer Container;
        private IRegionManager RegionManager;
        private IFCUTransactionServices FCUTransactionServices;

        public FCUDetailsSummaryViewModel(IUnityContainer _Container, IRegionManager _RegionManager, IFCUTransactionServices _FCUTransactionServices)
        {
            Container = _Container;
            RegionManager = _RegionManager;
            FCUTransactionServices = _FCUTransactionServices;
        }
    }
}
