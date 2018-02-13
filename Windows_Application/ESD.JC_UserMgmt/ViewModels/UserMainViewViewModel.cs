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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ESD.JC_UserMgmt.ViewModels
{
    public class UserMainViewViewModel : BindableBase
    {
        private ICollectionView _Users;
        public ICollectionView Users
        {
            get { return _Users; }
            set { SetProperty(ref _Users, value); }
        }

        private User _SelectedUser;
        public User SelectedUser
        {
            get { return _SelectedUser; }
            set { SetProperty(ref _SelectedUser, value); }
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
                    _editUserCommand.RaiseCanExecuteChanged();
                    _deleteUserCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _AuthenticatedUser = string.Empty;
        public string AuthenticatedUser
        {
            get { return _AuthenticatedUser; }
            set { SetProperty(ref _AuthenticatedUser, value); }
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

        private const string userDetailsViewName = "UserDetailsView";
        private const string userOperationViewName = "UserOperationView";
        private const string userSummaryViewName = "UserSummaryView";

        private IEventAggregator EventAggregator;
        private IRegionManager RegionManager;
        private IUserServices UserServices;
        private DelegateCommand<object> _addUserCommand;
        private DelegateCommand<object> _editUserCommand;
        private DelegateCommand<object> _deleteUserCommand;
        private InteractionRequest<Confirmation> confirmDeleteInteractionRequest;

        public UserMainViewViewModel(IEventAggregator _EventAggregator, IRegionManager _RegionManager, IUserServices _UserServices)
        {
            RegionManager = _RegionManager;
            UserServices = _UserServices;
            EventAggregator = _EventAggregator;
            EventAggregator.GetEvent<AuthenticatedUserEvent>().Subscribe(InitAuthenticatedUser);

            OnLoadedCommand = new DelegateCommand(OnLoaded);
            OpenUserDetailsCommand = new DelegateCommand<User>(OpenUserDetails);
            _addUserCommand = new DelegateCommand<object>(AddUser);
            _editUserCommand = new DelegateCommand<object>(EditUser, CanEdit);
            _deleteUserCommand = new DelegateCommand<object>(DeleteUser, CanDelete);
            confirmDeleteInteractionRequest = new InteractionRequest<Confirmation>();
        }

        public DelegateCommand OnLoadedCommand { get; private set; }
        public ICommand OpenUserDetailsCommand { get; private set; }
        public ICommand AddUserCommand
        {
            get { return this._addUserCommand; }
        }
        public ICommand EditUserCommand
        {
            get { return this._editUserCommand; }
        }
        public ICommand DeleteUserCommand
        {
            get { return this._deleteUserCommand; }
        }
        public IInteractionRequest ConfirmDeleteInteractionRequest
        {
            get { return this.confirmDeleteInteractionRequest; }
        }

        private void OnLoaded()
        {
            Users = new ListCollectionView(UserServices.GetAll().ToList());

            CollectionViewSource.GetDefaultView(Users).Filter = UserFilter;
        }

        private void InitAuthenticatedUser(string user)
        {
            AuthenticatedUser = user;
        }

        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(FilterTextBox))
                return true;

            var user = (User)item;

            return (user.Username.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase) ||
                    user.Name.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase));
        }

        private void AddUser(object ignored)
        {
            var parameters = new NavigationParameters();
            parameters.Add("AuthenticatedUser", AuthenticatedUser);

            RegionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(userOperationViewName + parameters, UriKind.Relative));
        }

        private void EditUser(object ignored)
        {
            if (SelectedUser == null)
                return;

            var parameters = new NavigationParameters();
            parameters.Add("AuthenticatedUser", AuthenticatedUser);
            parameters.Add("ID", SelectedUser.ID);

            RegionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(userOperationViewName + parameters, UriKind.Relative));
        }

        private bool CanEdit(object ignored)
        {
            return SelectedUser != null;
        }

        private void DeleteUser(object ignored)
        {
            if (SelectedUser == null)
                return;

            this.confirmDeleteInteractionRequest.Raise(
                    new Confirmation
                    {
                        Content = "Are you confirm you want to remove this?",
                        Title = "Confirm"
                    },
                    c => { InteractionResultMessage = c.Confirmed ? InitDelete(SelectedUser.ID) : "NOT OK!"; });

            OnLoaded();
        }

        private bool CanDelete(object ignored)
        {
            if (SelectedUser == null)
                return false;

            if (SelectedUser.Role.RoleCode.Equals("ADMIN"))
                return false;

            return true;
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

            return "OK!";
        }

        private void OpenUserDetails(User user)
        {
            var parameters = new NavigationParameters();
            parameters.Add("AuthenticatedUser", AuthenticatedUser);
            parameters.Add("ID", SelectedUser.ID);

            this.RegionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(userDetailsViewName + parameters, UriKind.Relative));
        }
    }
}
