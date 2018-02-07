using DataLayer;
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
    [RegionMemberLifetime(KeepAlive = false)]
    public class RoleOperationViewModel : BindableBase, IConfirmNavigationRequest
    {
        #region Properties

        private Role _RoleData;
        public Role RoleData
        {
            get { return _RoleData; }
            set { SetProperty(ref _RoleData, value); }
        }

        private string _RoleCodeAlias;
        public string RoleCodeAlias
        {
            get { return _RoleCodeAlias; }
            set
            {
                SetProperty(ref _RoleCodeAlias, value);
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _IsEnabled;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                SetProperty(ref _IsEnabled, value);
                RaisePropertyChanged("IsEnabled");
            }
        }

        private string _SendState;
        public string SendState
        {
            get { return _SendState; }
            set { SetProperty(ref _SendState, value); }
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

        #endregion

        private const string NormalStateKey = "Normal";
        private const string SavingStateKey = "Saving";
        private const string SavedStateKey = "Saved";

        private IRegionNavigationJournal navigationJournal;
        private IRoleServices RoleServices;
        private InteractionRequest<Confirmation> confirmExitInteractionRequest;
        private DelegateCommand<object> _saveCommand;

        public RoleOperationViewModel(IRoleServices _RoleServices)
        {
            RoleServices = _RoleServices;
            SendState = NormalStateKey;

            CancelCommand = new DelegateCommand(Cancel);
            _saveCommand = new DelegateCommand<object>(Save, CanSave);
            confirmExitInteractionRequest = new InteractionRequest<Confirmation>();
        }

        public ICommand CancelCommand { get; private set; }
        public ICommand SaveCommand
        {
            get { return this._saveCommand; }
        }
        public IInteractionRequest ConfirmExitInteractionRequest
        {
            get { return this.confirmExitInteractionRequest; }
        }
       
        private void Save(object ignored)
        {
            SendState = SavingStateKey;

            var data = this.RoleData;

            if (string.IsNullOrEmpty(data.RoleName))
            {
                this.SendState = NormalStateKey;

                MessageBox.Show("Role Name is required.", "Notification", MessageBoxButton.OK);
                return;
            }
            else if (string.IsNullOrEmpty(data.Module))
            {
                this.SendState = NormalStateKey;

                MessageBox.Show("Module is required.", "Notification", MessageBoxButton.OK);
                return;
            }

            try
            {
                var role = RoleServices.GetRole(data.ID);
                if (role != null)
                {
                    role.RoleCode = RoleCodeAlias;
                    role.RoleName = data.RoleName;
                    role.Module = data.Module;
                    role.Description = !string.IsNullOrEmpty(data.Description) ? data.Description : string.Empty;
                    role.ModifiedOn = DateTime.Now;
                    role.ModifiedBy = AuthenticatedUser;
                }
                else
                {
                    role = new Role();
                    role.RoleCode = RoleCodeAlias;
                    role.RoleName = data.RoleName;
                    role.Module = data.Module;
                    role.Description = data.Description;
                    role.CreatedOn = DateTime.Now;
                    role.CreatedBy = AuthenticatedUser;
                    role.ModifiedOn = DateTime.Now;
                    role.ModifiedBy = AuthenticatedUser;
                }

                if (RoleServices.Save(role))
                {
                    this.SendState = SavedStateKey;
                    if (this.navigationJournal != null)
                    {
                        this.navigationJournal.GoBack();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Notification", MessageBoxButton.OK);

                this.SendState = NormalStateKey;
            }
        }

        private bool CanSave(object ignored)
        {
            return !string.IsNullOrEmpty(RoleCodeAlias);
        }

        private void Cancel()
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

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            if (this.SendState == NormalStateKey)
            {
                this.confirmExitInteractionRequest.Raise(
                    new Confirmation
                    {
                        Content = "Are you sure you want to navigate away from this window?",
                        Title = "Confirm"
                    },
                    c => { continuationCallback(c.Confirmed); });
            }
            else
            {
                continuationCallback(true);
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var roledata = new Role();

            AuthenticatedUser = (!string.IsNullOrEmpty((string)navigationContext.Parameters["AuthenticatedUser"]) ? 
                (string)navigationContext.Parameters["AuthenticatedUser"] : string.Empty);

            var id = GetRequestedRoleID(navigationContext);
            if (id.HasValue)
            {
                var role = RoleServices.GetRole(id.Value);
                if (role != null)
                {
                    if (!string.IsNullOrEmpty(RoleCodeAlias))
                    {
                        IsEnabled = false;
                    }

                    RoleCodeAlias = role.RoleCode;
                    roledata.ID = role.ID;
                    roledata.RoleCode = role.RoleCode;
                    roledata.RoleName = role.RoleName;
                    roledata.Module = role.Module;
                    roledata.Description = role.Description;
                    roledata.ModifiedOn = role.ModifiedOn;
                    roledata.ModifiedBy = role.ModifiedBy;
                }
            }
            else
            {
                IsEnabled = true;
            }

            this.RoleData = roledata;

            this.navigationJournal = navigationContext.NavigationService.Journal;
        }
    }
}
