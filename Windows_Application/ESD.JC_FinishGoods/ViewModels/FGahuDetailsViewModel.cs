using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Windows.Input;
using DataLayer;
using ESD.JC_FinishGoods.Services;
using Microsoft.Practices.Unity;
using Prism.Events;
using ESD.JC_Infrastructure.Events;

namespace ESD.JC_FinishGoods.ViewModels
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class FGahuDetailsViewModel : BindableBase, INavigationAware
    {
        private AHU _AHU;
        public AHU AHU
        {
            get { return _AHU; }
            set { SetProperty(ref _AHU, value); }
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
        private IAHUServices ahuServices;


        public FGahuDetailsViewModel(IUnityContainer _Container, IRegionManager _RegionManager, IEventAggregator _EventAggregator, IAHUServices _ahuServices)
        {
            Container = _Container;
            RegionManager = _RegionManager;
            EventAggregator = _EventAggregator;
            ahuServices = _ahuServices;

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
            if (AHU == null)
            {
                return true;
            }

            var requestedId = GetRequestedId(navigationContext);

            return requestedId.HasValue && requestedId.Value == AHU.ID;
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
                this.AHU = ahuServices.GetAHU(id.Value);

                if (AHU.SectionReceived == null)
                    AHU.ShipStatus = false;
                else
                {
                    if (AHU.SectionReceived == AHU.Section)
                        AHU.ShipStatus = true;
                    else if (AHU.SectionReceived < AHU.Section)
                        AHU.ShipStatus = null;
                }

                this.EventAggregator.GetEvent<AHUSelectedEvent>().Publish(id.Value);
            }

            this.navigationJournal = navigationContext.NavigationService.Journal;
        }
    }
}
