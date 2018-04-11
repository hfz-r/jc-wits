using DataLayer;
using ESD.JC_GoodsReceive.Services;
using Prism.Mvvm;
using Prism.Regions;
using Microsoft.Practices.Unity;

namespace ESD.JC_GoodsReceive.ViewModels
{
    public class GRDetailsSummaryViewModel : BindableBase
    {
        public string ViewName
        {
            get { return "Transaction Summary"; }
        }

        private GoodsReceive _summryObj;
        public GoodsReceive summryObj
        {
            get { return _summryObj; }
            set
            {
                SetProperty(ref _summryObj, value);
                if (_summryObj != null)
                {
                    RemainingQty = _summryObj.Quantity - _summryObj.QtyReceived.GetValueOrDefault();
                }
            }
        }

        private decimal _RemainingQty;
        public decimal RemainingQty
        {
            get { return System.Math.Round(_RemainingQty, 2); }
            set
            {
                SetProperty(ref _RemainingQty, value);
                RaisePropertyChanged("RemainingQty");
            }
        }

        private IUnityContainer Container;
        private IRegionManager RegionManager;
        private IGRTransactionServices GRTransactionServices;

        public GRDetailsSummaryViewModel(IUnityContainer _Container, IRegionManager _RegionManager, IGRTransactionServices _GRTransactionServices)
        {
            Container = _Container;
            RegionManager = _RegionManager;
            GRTransactionServices = _GRTransactionServices;
        }
    }
}
