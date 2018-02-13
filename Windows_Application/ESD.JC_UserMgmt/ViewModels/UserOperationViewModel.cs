using DataLayer;
using ESD.JC_Infrastructure;
using ESD.JC_RoleMgmt.Services;
using ESD.JC_UserMgmt.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace ESD.JC_UserMgmt.ViewModels
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class UserOperationViewModel : BindableBase, IConfirmNavigationRequest
    {
        #region Properties

        private User _UserData;
        public User UserData
        {
            get { return _UserData; }
            set { SetProperty(ref _UserData, value); }
        }

        private string _UsernameAlias;
        public string UsernameAlias
        {
            get { return _UsernameAlias; }
            set
            {
                SetProperty(ref _UsernameAlias, value);
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        private string _PasswordLabel;
        public string PasswordLabel
        {
            get { return _PasswordLabel; }
            set
            {
                SetProperty(ref _PasswordLabel, value);
                RaisePropertyChanged("_PasswordLabel");
            }
        }

        private Role _SelectedRole;
        public Role SelectedRole
        {
            get { return _SelectedRole; }
            set { SetProperty(ref _SelectedRole, value); }
        }

        private ICollection<Role> _RoleList;
        public ICollection<Role> RoleList
        {
            get { return _RoleList; }
            set { SetProperty(ref _RoleList, value); }
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

        public static string Password = string.Empty;

        #endregion

        private const string NormalStateKey = "Normal";
        private const string SavingStateKey = "Saving";
        private const string SavedStateKey = "Saved";

        private IRegionNavigationJournal navigationJournal;
        private IUserServices UserServices;
        private IRoleServices RoleServices;
        private InteractionRequest<Confirmation> confirmExitInteractionRequest;
        private DelegateCommand<object> _saveCommand;

        public UserOperationViewModel(IUserServices _UserServices, IRoleServices _RoleServices)
        {
            UserServices = _UserServices;
            RoleServices = _RoleServices;
            SendState = NormalStateKey;

            OnLoadedCommand = new DelegateCommand(OnLoaded);
            CancelCommand = new DelegateCommand(Cancel);
            _saveCommand = new DelegateCommand<object>(Save, CanSave);
            confirmExitInteractionRequest = new InteractionRequest<Confirmation>();
        }

        public DelegateCommand OnLoadedCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand SaveCommand
        {
            get { return this._saveCommand; }
        }
        public IInteractionRequest ConfirmExitInteractionRequest
        {
            get { return this.confirmExitInteractionRequest; }
        }

        private void OnLoaded()
        {
            var roles = RoleServices.GetAll();
            if (roles.Count() > 0)
            {
                RoleList = new ObservableCollection<Role>();

                foreach (var r in roles)
                {
                    RoleList.Add(new Role
                    {
                        ID = r.ID,
                        RoleCode = r.RoleCode
                    });
                }
                SelectedRole = RoleList.Where(x => x.ID == UserData.RoleID).FirstOrDefault();
            }
        }

        private bool CanSave(object arg)
        {
            return !string.IsNullOrEmpty(_UsernameAlias);
        }

        private void Save(object ignored)
        {
            SendState = SavingStateKey;

            var data = this.UserData;

            if (string.IsNullOrEmpty(UsernameAlias))
            {
                this.SendState = NormalStateKey;

                MessageBox.Show("User Name is required.", "Notification", MessageBoxButton.OK);
                return;
            }
            else if (string.IsNullOrEmpty(Password))
            {
                this.SendState = NormalStateKey;

                MessageBox.Show("Password is required.", "Notification", MessageBoxButton.OK);
                return;
            }
            else if (string.IsNullOrEmpty(data.Name))
            {
                this.SendState = NormalStateKey;

                MessageBox.Show("Name is required.", "Notification", MessageBoxButton.OK);
                return;
            }
            else if (SelectedRole == null)
            {
                this.SendState = NormalStateKey;

                MessageBox.Show("Role is required.", "Notification", MessageBoxButton.OK);
                return;
            }

            try
            {
                bool ok = false;
                if (data.ID != 0)
                {
                    var updateObj = Update(data);
                    ok = UserServices.Save(updateObj, "Update");
                }
                else
                {
                    var newObj = Add(data);
                    ok = UserServices.Save(newObj, "Save");
                }

                if (ok)
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

        private User Add(User data)
        {
            return new User
            {
                Username = UsernameAlias,
                Password = HashConverter.CalculateHash(Password, UsernameAlias),
                RoleID = SelectedRole.ID,
                Name = data.Name,
                Email = data.Email,
                Address = data.Address,
                CreatedOn = DateTime.Now,
                CreatedBy = AuthenticatedUser,
                ModifiedOn = DateTime.Now,
                ModifiedBy = AuthenticatedUser
            };
        }

        private User Update(User data)
        {
            var userUpdate = UserServices.GetUser(data.ID);
            if (userUpdate != null)
            {
                userUpdate.Username = UsernameAlias;
                userUpdate.Password = HashConverter.CalculateHash(Password, UsernameAlias);
                userUpdate.Role = SelectedRole;
                userUpdate.RoleID = SelectedRole.ID;
                userUpdate.Name = data.Name;
                userUpdate.Email = data.Email;
                userUpdate.Address = data.Address;
                userUpdate.ModifiedOn = DateTime.Now;
                userUpdate.ModifiedBy = AuthenticatedUser;
            }
            return userUpdate;
        }

        private void Cancel()
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
            var userdata = new User();

            AuthenticatedUser = (!string.IsNullOrEmpty((string)navigationContext.Parameters["AuthenticatedUser"]) ?
                (string)navigationContext.Parameters["AuthenticatedUser"] : string.Empty);

            var ID = GetRequestedUserID(navigationContext);
            if (ID.HasValue)
            {
                var user = UserServices.GetUser(ID.Value);
                if (user != null)
                {
                    PasswordLabel = "New Password: ";

                    UsernameAlias = user.Username;
                    userdata.ID = user.ID;
                    userdata.Name = user.Name;
                    userdata.Username = user.Username;
                    userdata.Password = user.Password;
                    userdata.RoleID = user.RoleID;
                    userdata.Email = user.Email;
                    userdata.Address = user.Address;
                    userdata.ModifiedOn = user.ModifiedOn;
                    userdata.ModifiedBy = user.ModifiedBy;
                }
            }
            else
            {
                PasswordLabel = "Password: ";
            }

            this.UserData = userdata;

            this.navigationJournal = navigationContext.NavigationService.Journal;
        }
    }
}
