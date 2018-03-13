using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Windows.Input;
using DataLayer;
using ESD.JC_FinishGoods.Services;
using Microsoft.Practices.Unity;
using Prism.Events;

namespace ESD.JC_FinishGoods.ViewModels
{
    public class FGfcuDetailsViewModel : BindableBase, INavigationAware
    {
        private FCU _FCU;
        public FCU FCU
        {
            get { return _FCU; }
            set { SetProperty(ref _FCU, value); }
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
        private IUnityContainer Container;
        private IRegionManager RegionManager;
        private IEventAggregator EventAggregator;
        private IFCUServices fcuServices;


        public FGfcuDetailsViewModel(IUnityContainer _Container, IRegionManager _RegionManager, IEventAggregator _EventAggregator, IFCUServices _fcuServices)
        {
            Container = _Container;
            RegionManager = _RegionManager;
            EventAggregator = _EventAggregator;
            fcuServices = _fcuServices;

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

        private long? GetRequestedId(NavigationContext navigationContext)
        {
            var id = navigationContext.Parameters["ID"];
            if (id != null)
            {
                return long.Parse(id.ToString());
            }

            return null;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            if (FCU == null)
            {
                return true;
            }

            var requestedId = GetRequestedId(navigationContext);

            return requestedId.HasValue && requestedId.Value == FCU.ID;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            AuthenticatedUser = (!string.IsNullOrEmpty((string)navigationContext.Parameters["AuthenticatedUser"]) ?
                (string)navigationContext.Parameters["AuthenticatedUser"] : string.Empty);

            var id = GetRequestedId(navigationContext);
            if (id.HasValue)
            {
                this.FCU = fcuServices.GetFCU(id.Value);
            }

            this.navigationJournal = navigationContext.NavigationService.Journal;
        }
    }
}
