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

        private FCU _summryObj;
        public FCU summryObj
        {
            get { return _summryObj; }
            set
            {
                SetProperty(ref _summryObj, value);
                if (_summryObj != null)
                {
                    RemainingQty = _summryObj.Qty - _summryObj.QtyReceived.GetValueOrDefault();

                    CountCountries = _summryObj.FCUTransactions.Where(c => c.CountryID.HasValue).Count();
                    CountLocations = _summryObj.FCUTransactions.Where(l => l.LocationID.HasValue).Count();
                }

                RaisePropertyChanged("RemainingQty");
                RaisePropertyChanged("CountCountries");
                RaisePropertyChanged("CountLocations");
            }
        }

        private decimal _RemainingQty = 0;
        public decimal RemainingQty
        {
            get { return _RemainingQty; }
            set { SetProperty(ref _RemainingQty, value); }
        }

        private int _CountCountries = 0;
        public int CountCountries
        {
            get { return _CountCountries; }
            set { SetProperty(ref _CountCountries, value); }
        }

        private int _CountLocations = 0;
        public int CountLocations
        {
            get { return _CountLocations; }
            set { SetProperty(ref _CountLocations, value); }
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
