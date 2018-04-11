using DataLayer;
using ESD.JC_Infrastructure.Events;
using ESD.JC_UserMgmt.ModelsExt;
using ESD.JC_UserMgmt.Services;
using ESD.JC_RoleMgmt.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ESD.JC_Infrastructure;
using System.Text.RegularExpressions;

namespace ESD.JC_UserMgmt.ViewModels
{
    public class UserMainViewModel : BindableBase
    {
        private ObservableCollection<UserExt> _Users;
        public ObservableCollection<UserExt> Users
        {
            get { return _Users; }
            set { SetProperty(ref _Users, value); }
        }

        private UserExt _SelectedUser;
        public UserExt SelectedUser
        {
            get { return _SelectedUser; }
            set { SetProperty(ref _SelectedUser, value); }
        }

        private ObservableCollection<RoleExt> _Roles;
        public ObservableCollection<RoleExt> Roles
        {
            get { return _Roles; }
            set { SetProperty(ref _Roles, value); }
        }

        private DataGridCellInfo _cellInfo;
        public DataGridCellInfo CellInfo
        {
            get { return _cellInfo; }
            set
            {
                _cellInfo = value;
                if (_cellInfo != null)
                {
                    _deleteUserCommand.RaiseCanExecuteChanged();
                    _saveUserCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _AuthenticatedUser = string.Empty;
        public string AuthenticatedUser
        {
            get { return _AuthenticatedUser; }
            set { SetProperty(ref _AuthenticatedUser, value); }
        }

        private string _FilterTextBox;
        public string FilterTextBox
        {
            get { return _FilterTextBox; }
            set
            {
                SetProperty(ref _FilterTextBox, value);
                if (Users != null)
                    CollectionViewSource.GetDefaultView(Users).Refresh();
            }
        }

        private int _ItemCount;
        public int ItemCount
        {
            get { return _ItemCount; }
            set { SetProperty(ref _ItemCount, value); }
        }

        private const string userDetailsViewName = "UserDetailsView";

        private IEventAggregator EventAggregator;
        private IRegionManager RegionManager;
        private IUserServices UserServices;
        private IRoleServices RoleServices;
        private DelegateCommand<object> _saveUserCommand;
        private DelegateCommand<object> _deleteUserCommand;
        private InteractionRequest<Confirmation> _interactionRequest;

        public UserMainViewModel(IEventAggregator _EventAggregator, IRegionManager _RegionManager, IUserServices _UserServices, IRoleServices _RoleServices)
        {
            RegionManager = _RegionManager;
            UserServices = _UserServices;
            RoleServices = _RoleServices;
            EventAggregator = _EventAggregator;
            EventAggregator.GetEvent<AuthenticatedUserEvent>().Subscribe(InitAuthenticatedUser);

            OnLoadedCommand = new DelegateCommand(OnLoaded);
            OpenUserDetailsCommand = new DelegateCommand<UserExt>(OpenUserDetails);
            _saveUserCommand = new DelegateCommand<object>(SaveUser, CanSave);
            _deleteUserCommand = new DelegateCommand<object>(DeleteUser, CanDelete);
            _interactionRequest = new InteractionRequest<Confirmation>();
        }

        public DelegateCommand OnLoadedCommand { get; private set; }
        public ICommand OpenUserDetailsCommand { get; private set; }
        public ICommand SaveUserCommand
        {
            get { return this._saveUserCommand; }
        }
        public ICommand DeleteUserCommand
        {
            get { return this._deleteUserCommand; }
        }
        public IInteractionRequest InteractionRequest
        {
            get { return this._interactionRequest; }
        }

        private void OnLoaded()
        {
            Users = new ObservableCollection<UserExt>();
            Users.CollectionChanged += Users_CollectionChanged;

            foreach (var usr in UserServices.GetAll(true))
            {
                Users.Add(new UserExt
                {
                    ID = usr.ID,
                    Username = usr.Username,
                    Password = usr.Password.Substring(usr.Password.Length).PadLeft(usr.Password.Length, '*'),
                    Name = usr.Name,
                    Email = usr.Email,
                    Address = usr.Address,
                    RoleID = usr.RoleID,
                    ModifiedOn = usr.ModifiedOn,
                    ModifiedBy = usr.ModifiedBy,
                    IsEditable = usr.Username == AuthenticatedUser ? true : false
                });
            }

            PopulateRoleComboBox();

            CollectionViewSource.GetDefaultView(Users).Filter = UserFilter;

            Users = SequencingService.SetCollectionSequence(Users);
            RaisePropertyChanged("Users");
        }

        private void PopulateRoleComboBox()
        {
            Roles = new ObservableCollection<RoleExt>();

            foreach (var role in RoleServices.GetAll(false))
            {
                Roles.Add(new RoleExt
                {
                    ID = role.ID,
                    RoleCode = role.RoleCode
                });
            }
        }

        private void Users_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ItemCount = Users.Count;
            SequencingService.SetCollectionSequence(Users);
        }

        private void InitAuthenticatedUser(string user)
        {
            AuthenticatedUser = user;
        }

        private bool UserFilter(object item)
        {
            if (string.IsNullOrEmpty(FilterTextBox))
                return true;

            var user = (UserExt)item;

            return (user.Username.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase) ||
                    user.Name.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase));
        }

        private void OpenUserDetails(UserExt user)
        {
            var parameters = new NavigationParameters();
            parameters.Add("AuthenticatedUser", AuthenticatedUser);
            parameters.Add("ID", user.ID);

            this.RegionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(userDetailsViewName + parameters, UriKind.Relative));
        }

        private bool CanSave(object ignored)
        {
            if (SelectedUser != null && !string.IsNullOrEmpty(SelectedUser.Username) && 
                Users.Where(nm => nm.Username == SelectedUser.Username).Count() > 1)
                return false;
            if (SelectedUser != null && !string.IsNullOrEmpty(SelectedUser.Email) && 
                Users.Where(nm => nm.Email == SelectedUser.Email).Count() > 1)
                return false;

            return true;
        }

        private void SaveUser(object ignored)
        {
            this._interactionRequest.Raise(
                    new Confirmation
                    {
                        Content = "Confirm to save this?",
                        Title = "Confirm"
                    },
                    c =>
                    {
                        if (c.Confirmed)
                        {
                            if (InitSave())
                                OnLoaded();
                        }
                    });
        }

        private void DeleteUser(object ignored)
        {
            if (SelectedUser == null)
                return;

            if (SelectedUser.ID != 0)
            {
                this._interactionRequest.Raise(
                    new Confirmation
                    {
                        Content = "Confirm to remove this?",
                        Title = "Confirm"
                    },
                    c =>
                    {
                        if (c.Confirmed)
                        {
                            if (InitDelete(SelectedUser.ID))
                                OnLoaded();
                        }

                    });
            }
            else
            {
                if (!string.IsNullOrEmpty(SelectedUser.Username) &&
                      !string.IsNullOrEmpty(SelectedUser.Password) &&
                      SelectedUser.RoleID != 0 &&
                      !string.IsNullOrEmpty(SelectedUser.Address) &&
                      !string.IsNullOrEmpty(SelectedUser.Name) &&
                      !string.IsNullOrEmpty(SelectedUser.Email) &&
                      !string.IsNullOrEmpty(SelectedUser.Password))
                {
                    Users.Remove(SelectedUser);
                }
            }
        }

        private bool InitSave()
        {
            bool ok = false;

            List<UserExt> toSaveList = new List<UserExt>();
            List<UserExt> toUpdateList = new List<UserExt>();
            foreach (var usr in Users.ToList())
            {
                if (string.IsNullOrEmpty(usr.Username))
                {
                    MessageBox.Show("Username is required.");
                    return false;
                }
                else if (string.IsNullOrEmpty(usr.Password))
                {
                    MessageBox.Show("Password is required.");
                    return false;
                }
                else if (string.IsNullOrEmpty(usr.Name))
                {
                    MessageBox.Show("Name is required.");
                    return false;
                }
                else if (usr.RoleID == null)
                {
                    MessageBox.Show("Role is required.");
                    return false;
                }

                if (usr.ID == 0 && !string.IsNullOrEmpty(usr.Username))
                    toSaveList.Add(usr);
                else if (usr.ID != 0)
                    toUpdateList.Add(usr);
            }

            try
            {
                List<User> addObj = Add(toSaveList);
                if (addObj.Count() > 0)
                    ok = UserServices.Save(addObj, "Save");

                List<User> updateObj = Update(toUpdateList);
                if (updateObj.Count() > 0)
                    ok = UserServices.Save(updateObj, "Update");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Notification", MessageBoxButton.OK);
            }

            return ok;
        }

        private List<User> Add(List<UserExt> toSaveList)
        {
            List<User> addObj = new List<User>();

            foreach (var o in toSaveList)
            {
                var reason = new User
                {
                    Username = o.Username,
                    Password = HashConverter.CalculateHash(o.Password, o.Username),
                    Name = o.Name,
                    RoleID = o.RoleID,
                    Email = o.Email,
                    Address = o.Address,
                    CreatedOn = DateTime.Now,
                    CreatedBy = AuthenticatedUser,
                    ModifiedOn = o.ModifiedOn,
                    ModifiedBy = AuthenticatedUser
                };
                addObj.Add(reason);
            }
            return addObj;
        }

        private List<User> Update(List<UserExt> toUpdateList)
        {
            List<User> updateObj = new List<User>();

            foreach (var u in toUpdateList)
            {
                if (!u.Password.Contains("*"))
                {
                    var tempPass = HashConverter.CalculateHash(u.Password, u.Username);

                    var user = UserServices.GetUser(u.ID);
                    if (user != null && (user.Username != u.Username ||
                                         user.Name != u.Name ||
                                         user.RoleID != u.RoleID ||
                                         user.Email != u.Email ||
                                         user.Address != u.Address ||
                                         user.Password != tempPass))
                    {
                        user.Username = u.Username;
                        user.Password = tempPass;
                        user.Name = u.Name;
                        user.Role = RoleServices.GetRole(u.RoleID.GetValueOrDefault());
                        user.RoleID = u.RoleID;
                        user.Email = u.Email;
                        user.Address = u.Address;
                        user.ModifiedOn = DateTime.Now;
                        user.ModifiedBy = AuthenticatedUser;
                        updateObj.Add(user);
                    }
                }
            }
            return updateObj;
        }
        
        private bool CanDelete(object ignored)
        {
            if (SelectedUser == null)
                return true;
            if (SelectedUser.Username == AuthenticatedUser)
                return true;
            if (AuthenticatedUser == "sa")
                return true;
            if (CellInfo.Item.ToString().Contains("NewItemPlaceholder"))
                return true;

            return false;
        }

        private bool InitDelete(long ID)
        {
            bool ok = false;

            try
            {
                if (ID != 0)
                {
                    ok = UserServices.Delete(ID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Notification", MessageBoxButton.OK);
            }

            return ok;
        }
    }
}
