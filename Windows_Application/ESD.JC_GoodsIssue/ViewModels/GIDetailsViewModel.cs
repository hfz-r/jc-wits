using DataLayer;
using ESD.JC_GoodsIssue.Services;
using ESD.JC_Infrastructure.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using System.Windows;
using System.Windows.Input;

namespace ESD.JC_GoodsIssue.ViewModels
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class GIDetailsViewModel : BindableBase, INavigationAware
    {
        private GITransaction _GoodsIssue;
        public GITransaction GoodsIssue
        {
            get { return _GoodsIssue; }
            set { SetProperty(ref _GoodsIssue, value); }
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

        private object _TransferPROD;
        public object TransferPROD
        {
            get { return _TransferPROD; }
            set
            {
                SetProperty(ref _TransferPROD, value);
                RaisePropertyChanged("TransferPROD");
            }
        }

        private object _TransferPOST;
        public object TransferPOST
        {
            get { return _TransferPOST; }
            set
            {
                SetProperty(ref _TransferPOST, value);
                RaisePropertyChanged("TransferPOST");
            }
        }

        private IRegionNavigationJournal navigationJournal;
        private IRegionManager RegionManager;
        private IEventAggregator EventAggregator;
        private IGIServices GIServices;
        private InteractionRequest<Confirmation> confirmRemoveThisInteractionRequest;

        public GIDetailsViewModel(IRegionManager _RegionManager, IEventAggregator _EventAggregator, IGIServices _GIServices)
        {
            RegionManager = _RegionManager;
            EventAggregator = _EventAggregator;
            GIServices = _GIServices;

            GoBackCommand = new DelegateCommand(GoBack);
            confirmRemoveThisInteractionRequest = new InteractionRequest<Confirmation>();
        }

        public ICommand GoBackCommand { get; private set; }
        public IInteractionRequest ConfirmRemoveThisInteractionRequest
        {
            get { return this.confirmRemoveThisInteractionRequest; }
        }
        
        private void GoBack()
        {
            if (this.navigationJournal != null)
            {
                this.navigationJournal.GoBack();
            }
        }

        private long? GetRequestedGIID(NavigationContext navigationContext)
        {
            var uid = navigationContext.Parameters["ID"];
            if (uid != null)
            {
                return long.Parse(uid.ToString());
            }

            return null;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            if (GoodsIssue == null)
            {
                return true;
            }

            var requestedGIID = GetRequestedGIID(navigationContext);

            return requestedGIID.HasValue && requestedGIID.Value == GoodsIssue.ID;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            AuthenticatedUser = (!string.IsNullOrEmpty((string)navigationContext.Parameters["AuthenticatedUser"]) ?
                (string)navigationContext.Parameters["AuthenticatedUser"] : string.Empty);

            var id = GetRequestedGIID(navigationContext);
            if (id.HasValue)
            {
                this.GoodsIssue = GIServices.GetGI(id.Value.ToString());

                if (GoodsIssue.TransferType == "TRANSFER_POST")
                {
                    TransferPOST = true;
                    TransferPROD = null;
                }
                else if (GoodsIssue.TransferType == "TRANSFER_PROD")
                {
                    TransferPOST = null;
                    TransferPROD = true;
                }

                this.EventAggregator.GetEvent<GIUserSelectedEvent>().Publish(id.Value);
            }

            this.navigationJournal = navigationContext.NavigationService.Journal;
        }
    }
}
