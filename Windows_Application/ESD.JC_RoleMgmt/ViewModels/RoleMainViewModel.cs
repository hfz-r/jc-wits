using DataLayer;
using ESD.JC_Infrastructure;
using ESD.JC_Infrastructure.Events;
using ESD.JC_RoleMgmt.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ESD.JC_RoleMgmt.ViewModels
{
    public class RoleMainViewModel : BindableBase
    {
        private ICollectionView _Roles;
        public ICollectionView Roles
        {
            get { return _Roles; }
            set { SetProperty(ref _Roles, value); }
        }

        private Role _SelectedRole;
        public Role SelectedRole
        {
            get { return _SelectedRole; }
            set { SetProperty(ref _SelectedRole, value); }
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
                    _editRoleCommand.RaiseCanExecuteChanged();
                    _deleteRoleCommand.RaiseCanExecuteChanged();
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
                if (Roles != null)
                    CollectionViewSource.GetDefaultView(Roles).Refresh();
            }
        }

        private const string roleDetailsViewName = "RoleDetailsView";
        private const string roleOperationViewName = "RoleOperationView";

        public ObservableCollection<Role> roleCollection { get; private set; }

        private IEventAggregator EventAggregator;
        private IRegionManager RegionManager;
        private IRoleServices RoleServices;
        private DelegateCommand<object> _addRoleCommand;
        private DelegateCommand<object> _editRoleCommand;
        private DelegateCommand<object> _deleteRoleCommand;
        private InteractionRequest<Confirmation> confirmDeleteInteractionRequest;

        public RoleMainViewModel(IEventAggregator _EventAggregator, IRegionManager _RegionManager, IRoleServices _RoleServices)
        {
            RegionManager = _RegionManager;
            RoleServices = _RoleServices;
            EventAggregator = _EventAggregator;
            EventAggregator.GetEvent<AuthenticatedUserEvent>().Subscribe(InitAuthenticatedUser);

            OnLoadedCommand = new DelegateCommand(OnLoaded);
            OpenRoleDetailsCommand = new DelegateCommand<Role>(OpenRoleDetails);

            _addRoleCommand = new DelegateCommand<object>(AddRole);
            _editRoleCommand = new DelegateCommand<object>(EditRole, CanEdit);
            _deleteRoleCommand = new DelegateCommand<object>(DeleteRole, CanDelete);
            confirmDeleteInteractionRequest = new InteractionRequest<Confirmation>();
        }

        public DelegateCommand OnLoadedCommand { get; private set; }
        public ICommand OpenRoleDetailsCommand { get; private set; }
        public ICommand AddRoleCommand
        {
            get { return this._addRoleCommand; }
        }
        public ICommand EditRoleCommand
        {
            get { return this._editRoleCommand; }
        }
        public ICommand DeleteRoleCommand
        {
            get { return this._deleteRoleCommand; }
        }
        public IInteractionRequest ConfirmDeleteInteractionRequest
        {
            get { return this.confirmDeleteInteractionRequest; }
        }

        private void OnLoaded()
        {
            roleCollection = new ObservableCollection<Role>();
            foreach (var r in RoleServices.GetAll(true))
            {
                roleCollection.Add(r);
            }

            Roles = new ListCollectionView(roleCollection);
            Roles.SortDescriptions.Add(new SortDescription("RoleCode", ListSortDirection.Ascending));

            CollectionViewSource.GetDefaultView(Roles).Filter = RoleFilter;
        }

        private void InitAuthenticatedUser(string user)
        {
            AuthenticatedUser = user;
        }

        private bool RoleFilter(object item)
        {
            if (String.IsNullOrEmpty(FilterTextBox))
                return true;

            var role = (Role)item;

            return (role.RoleCode.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase) ||
                    role.RoleName.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase));
        }

        private void AddRole(object ignored)
        {
            var parameters = new NavigationParameters();
            parameters.Add("AuthenticatedUser", AuthenticatedUser);

            RegionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(roleOperationViewName + parameters, UriKind.Relative));
        }

        private void EditRole(object ignored)
        {
            if (SelectedRole == null)
                return;

            var parameters = new NavigationParameters();
            parameters.Add("AuthenticatedUser", AuthenticatedUser);
            parameters.Add("ID", SelectedRole.ID);

            this.RegionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(roleOperationViewName + parameters, UriKind.Relative));
        }

        private bool CanEdit(object ignored)
        {
            return SelectedRole != null;
        }

        private void DeleteRole(object ignored)
        {
            if (SelectedRole == null)
                return;

            this.confirmDeleteInteractionRequest.Raise(
                    new Confirmation
                    {
                        Content = "Confirm to remove this?",
                        Title = "Confirm"
                    },
                    c => { InteractionResultMessage = c.Confirmed ? InitDelete(SelectedRole.ID) : "NOT OK!"; });

            OnLoaded();
        }

        private bool CanDelete(object ignored)
        {
            if (SelectedRole == null)
                return false;
            if (SelectedRole.RoleCode == "ADMINISTRATOR")
                return false;

            var user = SelectedRole.Users.Where(x => x.Username == AuthenticatedUser).FirstOrDefault();
            if (user != null && SelectedRole.ID == user.RoleID)
                return false;

            return true;
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

            return "Role deleted.";
        }

        private void OpenRoleDetails(Role role)
        {
            var parameters = new NavigationParameters();
            parameters.Add("AuthenticatedUser", AuthenticatedUser);
            parameters.Add("ID", SelectedRole.ID);

            this.RegionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(roleDetailsViewName + parameters, UriKind.Relative));
        }
    }
}
