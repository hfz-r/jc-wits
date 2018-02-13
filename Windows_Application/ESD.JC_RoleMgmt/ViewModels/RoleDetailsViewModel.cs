using DataLayer;
using ESD.JC_Infrastructure;
using ESD.JC_RoleMgmt.Services;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Input;

namespace ESD.JC_RoleMgmt.ViewModels
{
    public class RoleDetailsViewModel : BindableBase, INavigationAware
    {
        private Role _Role;
        public Role Role
        {
            get { return _Role; }
            set { SetProperty(ref _Role, value); }
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

        private const string roleOperationViewName = "RoleOperationView";

        private IRegionNavigationJournal navigationJournal;
        private IRegionManager RegionManager;
        private IRoleServices RoleServices;
        private InteractionRequest<Confirmation> confirmRemoveThisInteractionRequest;

        public RoleDetailsViewModel(IRegionManager _RegionManager, IRoleServices _RoleServices)
        {
            RegionManager = _RegionManager;
            RoleServices = _RoleServices;

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
            if (Role == null)
                return;

            var parameters = new NavigationParameters();
            parameters.Add("AuthenticatedUser", AuthenticatedUser);
            parameters.Add("ID", Role.ID);

            this.RegionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(roleOperationViewName + parameters, UriKind.Relative));
        }

        private void RemoveThis()
        {
            if (Role == null)
                return;

            this.confirmRemoveThisInteractionRequest.Raise(
                    new Confirmation
                    {
                        Content = "Are you confirm you want to remove this?",
                        Title = "Confirm"
                    },
                    c => { InteractionResultMessage = c.Confirmed ? InitDelete(Role.ID) : "NOT OK!"; });

            GoBack();
        }

        private string InitDelete(long ID)
        {
            try
            {
                if (ID != 0)
                {
                    RoleServices.Delete(ID);
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
            if (Role == null)
                return false;

            if (Role.RoleCode.Equals("ADMIN"))
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

        private long? GetRequestedRoleID(NavigationContext navigationContext)
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
            if (Role == null)
            {
                return true;
            }

            var requestedRoleID = GetRequestedRoleID(navigationContext);

            return requestedRoleID.HasValue && requestedRoleID.Value == Role.ID;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            AuthenticatedUser = (!string.IsNullOrEmpty((string)navigationContext.Parameters["AuthenticatedUser"]) ?
                (string)navigationContext.Parameters["AuthenticatedUser"] : string.Empty);

            var id = GetRequestedRoleID(navigationContext);
            if (id.HasValue)
            {
                this.Role = RoleServices.GetRole(id.Value);
            }

            this.navigationJournal = navigationContext.NavigationService.Journal;
        }
    }
}
