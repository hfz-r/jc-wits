using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Windows.Input;
using DataLayer;
using ESD.JC_GoodsReceive.Services;
using Microsoft.Practices.Unity;
using ESD.JC_Infrastructure.Events;
using Prism.Events;
using System.Linq;
using ESD.JC_GoodsReceive.Notifications;
using Prism.Interactivity.InteractionRequest;
using System.Windows;

namespace ESD.JC_GoodsReceive.ViewModels
{
    [RegionMemberLifetime(KeepAlive = false)]
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
        private IUnityContainer Container;
        private IRegionManager RegionManager;
        private IEventAggregator EventAggregator;
        private IGRServices GRServices;
        private IGRTransactionServices GRTrnxServices;

        private InteractionRequest<ObjectSelectionNotification> eunSetDetailsRequest;

        public GRDetailsViewModel(IUnityContainer _Container, IRegionManager _RegionManager, IEventAggregator _EventAggregator, 
            IGRServices _GRServices, IGRTransactionServices _GRTrnxServices)
        {
            Container = _Container;
            RegionManager = _RegionManager;
            EventAggregator = _EventAggregator;
            GRServices = _GRServices;
            GRTrnxServices = _GRTrnxServices;

            GoBackCommand = new DelegateCommand(GoBack);
            EunSetDetails = new DelegateCommand<GoodsReceive>(EunDetails);
            eunSetDetailsRequest = new InteractionRequest<ObjectSelectionNotification>();
        }

        public ICommand GoBackCommand { get; private set; }
        public ICommand EunSetDetails { get; private set; }
        public IInteractionRequest EunSetDetailsRequest
        {
            get { return this.eunSetDetailsRequest; }
        }

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

                this.EventAggregator.GetEvent<UserSelectedEvent>().Publish(grid.Value);
            }

            this.navigationJournal = navigationContext.NavigationService.Journal;
        }

        private void EunDetails(GoodsReceive obj)
        {
            ObjectSelectionNotification notification = new ObjectSelectionNotification();
            notification.ParentItem = obj;
            notification.Title = "Unit Kilogram (KG) Child Window";
            notification.AuthenticatedUser = AuthenticatedUser;

            var kgs = GRServices.GetEunKG(obj.ID).EunKGs;
            if (kgs.Count() > 0)
            {
                foreach (var item in kgs)
                    notification.Items.Add(item);
            }

            this.eunSetDetailsRequest.Raise(
                notification,
                returned =>
                {
                    //if (returned != null && returned.Confirmed && returned.SelectedItem != null)
                    //{
                    //    if (Save())
                    //        OnLoaded();
                    //}
                    if (returned != null && returned.Confirmed)
                    {
                        MessageBox.Show("Child details successfully been added.", "Success", MessageBoxButton.OK);
                    }
                });
        }

    }
}
