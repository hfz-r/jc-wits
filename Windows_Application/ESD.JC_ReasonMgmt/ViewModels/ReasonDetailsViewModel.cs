using DataLayer;
using ESD.JC_Infrastructure;
using ESD.JC_ReasonMgmt.Services;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Input;

namespace ESD.JC_ReasonMgmt.ViewModels
{
    public class ReasonDetailsViewModel : BindableBase, INavigationAware
    {
        private Reason _Reason;
        public Reason Reason
        {
            get { return _Reason; }
            set { SetProperty(ref _Reason, value); }
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

        private string _InteractionResultMessage;
        public string InteractionResultMessage
        {
            get { return _InteractionResultMessage; }
            set
            {
                SetProperty(ref _InteractionResultMessage, value);
                this.RaisePropertyChanged("InteractionResultMessage");
            }
        }

        private const string reasonOperationViewName = "ReasonOperationView";

        private IRegionNavigationJournal navigationJournal;
        private IRegionManager RegionManager;
        private IReasonServices ReasonServices;
        private InteractionRequest<Confirmation> confirmRemoveThisInteractionRequest;

        public ReasonDetailsViewModel(IRegionManager _RegionManager, IReasonServices _ReasonServices)
        {
            RegionManager = _RegionManager;
            ReasonServices = _ReasonServices;

            EditThisCommand = new DelegateCommand(EditThis);
            RemoveThisCommand = new DelegateCommand(RemoveThis, CanRemove);
            GoBackCommand = new DelegateCommand(GoBack);
            confirmRemoveThisInteractionRequest = new InteractionRequest<Confirmation>();
        }

        public ICommand EditThisCommand { get; private set; }
        public ICommand RemoveThisCommand { get; private set; }
        public ICommand GoBackCommand { get; private set; }
        public IInteractionRequest ConfirmRemoveThisInteractionRequest
        {
            get { return this.confirmRemoveThisInteractionRequest; }
        }

        private void EditThis()
        {
            if (Reason == null)
                return;

            var parameters = new NavigationParameters();
            parameters.Add("AuthenticatedUser", AuthenticatedUser);
            parameters.Add("ID", Reason.ID);

            this.RegionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(reasonOperationViewName + parameters, UriKind.Relative));
        }

        private void RemoveThis()
        {
            if (Reason == null)
                return;

            this.confirmRemoveThisInteractionRequest.Raise(
                    new Confirmation
                    {
                        Content = "Are you confirm you want to remove this?",
                        Title = "Confirm"
                    },
                    c => { InteractionResultMessage = c.Confirmed ? InitDelete(Reason.ID) : "NOT OK!"; });

            GoBack();
        }

        private string InitDelete(long? ID)
        {
            try
            {
                if (ID.HasValue)
                {
                    ReasonServices.Delete(ID.GetValueOrDefault());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Notification", MessageBoxButton.OK);
            }

            return "OK!";
        }

        private bool CanRemove()
        {
            if (Reason == null)
                return false;

            //if (Reason.ReasonCode.Equals("ADMIN"))
            //    return false;

            return true;
        }

        private void GoBack()
        {
            if (this.navigationJournal != null)
            {
                this.navigationJournal.GoBack();
            }
        }

        private long? GetRequestedReasonID(NavigationContext navigationContext)
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
            if (Reason == null)
            {
                return true;
            }

            var requestedReasonID = GetRequestedReasonID(navigationContext);

            return requestedReasonID.HasValue && requestedReasonID.Value == Reason.ID;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            AuthenticatedUser = (!string.IsNullOrEmpty((string)navigationContext.Parameters["AuthenticatedUser"]) ?
                (string)navigationContext.Parameters["AuthenticatedUser"] : string.Empty);

            var id = GetRequestedReasonID(navigationContext);
            if (id.HasValue)
            {
                this.Reason = ReasonServices.GetReason(id.Value);
            }

            this.navigationJournal = navigationContext.NavigationService.Journal;
        }
    }
}
