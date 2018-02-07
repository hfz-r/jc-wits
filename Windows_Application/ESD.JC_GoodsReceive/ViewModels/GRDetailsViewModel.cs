using DataLayer;
using ESD.JC_GoodsReceive.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Windows.Input;

namespace ESD.JC_GoodsReceive.ViewModels
{
    public class GRDetailsViewModel : BindableBase, INavigationAware
    {
        private GoodsReceive _GoodReceive;
        public GoodsReceive GoodReceive
        {
            get { return _GoodReceive; }
            set { SetProperty(ref _GoodReceive, value); }
        }

        private string _AuthenticatedUser;
        public string AuthenticatedUser
        {
            get { return _AuthenticatedUser; }
            set
            {
                SetProperty(ref _AuthenticatedUser, value);
                RaisePropertyChanged("AuthenticatedUser");
            }
        }

        private IRegionNavigationJournal navigationJournal;
        private IRegionManager RegionManager;
        private IGRServices GRServices;

        public GRDetailsViewModel(IRegionManager _RegionManager, IGRServices _GRServices)
        {
            RegionManager = _RegionManager;
            GRServices = _GRServices;

            GoBackCommand = new DelegateCommand(GoBack);
        }

        public ICommand GoBackCommand { get; private set; }

        private void GoBack()
        {
            if (this.navigationJournal != null)
            {
                this.navigationJournal.GoBack();
            }
        }

        private string GetRequestedPONo(NavigationContext navigationContext)
        {
            var pono = navigationContext.Parameters["PurchaseOrder"];
            if (pono != null)
            {
                return pono.ToString();
            }

            return string.Empty;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            if (GoodReceive == null)
            {
                return true;
            }

            var requestedPONo = GetRequestedPONo(navigationContext);

            return !string.IsNullOrEmpty(requestedPONo) == (requestedPONo == GoodReceive.PurchaseOrder);
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            AuthenticatedUser = (!string.IsNullOrEmpty((string)navigationContext.Parameters["AuthenticatedUser"]) ?
                (string)navigationContext.Parameters["AuthenticatedUser"] : string.Empty);

            var po_no = GetRequestedPONo(navigationContext);
            if (!string.IsNullOrEmpty(po_no))
            {
                this.GoodReceive = GRServices.GetGR(po_no);
            }

            this.navigationJournal = navigationContext.NavigationService.Journal;
        }
    }
}
