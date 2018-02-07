using DataLayer;
using ESD.JC_Infrastructure;
using ESD.JC_UserMgmt.Services;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Input;

namespace ESD.JC_UserMgmt.ViewModels
{
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
        private IUserServices UserServices;
        private InteractionRequest<Confirmation> confirmRemoveThisInteractionRequest;

        public UserDetailsViewModel(IRegionManager _RegionManager, IUserServices _UserServices)
        {
            RegionManager = _RegionManager;
            UserServices = _UserServices;

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
            if (User == null)
                return;

            var parameters = new NavigationParameters();
            parameters.Add("AuthenticatedUser", AuthenticatedUser);
            parameters.Add("ID", User.ID);

            this.RegionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(userOperationViewName + parameters, UriKind.Relative));
        }

        private void RemoveThis()
        {
            if (User == null)
                return;

            this.confirmRemoveThisInteractionRequest.Raise(
                    new Confirmation
                    {
                        Content = "Are you confirm you want to remove this?",
                        Title = "Confirm"
                    },
                    c => { InteractionResultMessage = c.Confirmed ? InitDelete(User.ID) : "NOT OK!"; });

            GoBack();
        }

        private string InitDelete(long? ID)
        {
            try
            {
                if (ID.HasValue)
                {
                    UserServices.Delete(ID);
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
            if (User == null)
                return false;

            if (User.Role.RoleCode.Equals("ADMIN"))
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
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            AuthenticatedUser = (!string.IsNullOrEmpty((string)navigationContext.Parameters["AuthenticatedUser"]) ?
                (string)navigationContext.Parameters["AuthenticatedUser"] : string.Empty);

            var ID = GetRequestedUserID(navigationContext);
            if (ID.HasValue)
            {
                this.User = UserServices.GetUser(ID.Value);
            }

            this.navigationJournal = navigationContext.NavigationService.Journal;
        }
    }
}
