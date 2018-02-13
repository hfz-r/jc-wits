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

        private long? GetRequestedGRID(NavigationContext navigationContext)
        {
            var grid = navigationContext.Parameters["ID"];
            if (grid != null)
            {
                return long.Parse(grid.ToString());
            }

            return null;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            if (GoodReceive == null)
            {
                return true;
            }

            var requestedGRID = GetRequestedGRID(navigationContext);

            return requestedGRID.HasValue && requestedGRID.Value == GoodReceive.ID;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            AuthenticatedUser = (!string.IsNullOrEmpty((string)navigationContext.Parameters["AuthenticatedUser"]) ?
                (string)navigationContext.Parameters["AuthenticatedUser"] : string.Empty);

            var grid = GetRequestedGRID(navigationContext);
            if (grid.HasValue)
            {
                this.GoodReceive = GRServices.GetGR(grid.Value);
            }

            this.navigationJournal = navigationContext.NavigationService.Journal;
        }
    }
}
