using DataLayer;
using ESD.JC_Infrastructure;
using ESD.JC_Infrastructure.Events;
using ESD.JC_UserMgmt.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Input;

namespace ESD.JC_UserMgmt.ViewModels
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class UserDetailsViewModel : BindableBase, INavigationAware
    {
        private User _User;
        public User User
        {
            get { return _User; }
            set { SetProperty(ref _User, value); }
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

        private const string userOperationViewName = "UserOperationView";

        private IRegionNavigationJournal navigationJournal;
        private IRegionManager RegionManager;
        private IEventAggregator EventAggregator;
        private IUserServices UserServices;
        private InteractionRequest<Confirmation> confirmRemoveThisInteractionRequest;

        public UserDetailsViewModel(IRegionManager _RegionManager, IUserServices _UserServices, IEventAggregator _EventAggregator)
        {
            RegionManager = _RegionManager;
            EventAggregator = _EventAggregator;
            UserServices = _UserServices;

            RemoveThisCommand = new DelegateCommand(RemoveThis, CanRemove);
            GoBackCommand = new DelegateCommand(GoBack);
            confirmRemoveThisInteractionRequest = new InteractionRequest<Confirmation>();
        }

        public ICommand RemoveThisCommand { get; private set; }
        public ICommand GoBackCommand { get; private set; }
        public IInteractionRequest ConfirmRemoveThisInteractionRequest
        {
            get { return this.confirmRemoveThisInteractionRequest; }
        }

        private void RemoveThis()
        {
            if (User == null)
                return;

            this.confirmRemoveThisInteractionRequest.Raise(
                    new Confirmation
                    {
                        Content = "Confirm to remove this?",
                        Title = "Confirm"
                    },
                    c => { InteractionResultMessage = c.Confirmed ? InitDelete(User.ID) : "NOT OK!"; });

            GoBack();
        }

        private string InitDelete(long ID)
        {
            try
            {
                if (ID != 0)
                {
                    UserServices.Delete(ID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Notification", MessageBoxButton.OK);
            }

            return "User deleted.";
        }

        private bool CanRemove()
        {
            if (User == null)
                return false;
            if (User.Username == AuthenticatedUser)
                return false;
            if (User.Role.RoleCode == "ADMINISTRATOR")
                return false;

            return true;
        }

        private void GoBack()
        {
            if (this.navigationJournal != null)
            {
                this.navigationJournal.GoBack();
            }
        }

        private long? GetRequestedUserID(NavigationContext navigationContext)
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
            if (User == null)
            {
                return true;
            }

            var requestedID = GetRequestedUserID(navigationContext);

            return requestedID.HasValue && requestedID.Value == User.ID;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            navigationContext.NavigationService.Region.RegionManager.Regions.Remove(RegionNames.TabRegionRole);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            AuthenticatedUser = (!string.IsNullOrEmpty((string)navigationContext.Parameters["AuthenticatedUser"]) ?
                (string)navigationContext.Parameters["AuthenticatedUser"] : string.Empty);

            var ID = GetRequestedUserID(navigationContext);
            if (ID.HasValue)
            {
                this.User = UserServices.GetUser(ID.Value);

                this.EventAggregator.GetEvent<UserSelectedEvent>().Publish(ID.Value);
            }

            this.navigationJournal = navigationContext.NavigationService.Journal;
        }
    }
}
