using DataLayer;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Events;
using System.Collections.ObjectModel;
using ESD.JC_RoleMgmt.Services;
using System.Windows.Input;
using Prism.Commands;
using System;
using ESD.JC_Infrastructure;
using System.Threading;

namespace ESD.JC_RoleMgmt.ViewModels
{
    public class RoleDetailsUsersViewModel : BindableBase
    {
        public string ViewName
        {
            get { return "Users List"; }
        }

        #region Properties

        private ObservableCollection<User> _userCollection;
        public ObservableCollection<User> userCollection
        {
            get { return _userCollection; }
            set
            {
                SetProperty(ref _userCollection, value);
                RaisePropertyChanged("userCollection");
            }
        }

        public string AuthenticatedUser
        {
            get
            {
                if (IsAuthenticated)
                    return Thread.CurrentPrincipal.Identity.Name;

                return "Unauthorized User";
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return Thread.CurrentPrincipal.Identity.IsAuthenticated;
            }
        }

        #endregion Properties

        private const string userDetailsViewName = "UserDetailsView";

        private IRegionManager regionManager;
        private IEventAggregator eventAggregator;
        private IRoleServices roleServices;

        public RoleDetailsUsersViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IRoleServices roleServices)
        {
            this.regionManager = regionManager;
            this.roleServices = roleServices;
            this.eventAggregator = eventAggregator;

            ViewUserDetailsCommand = new DelegateCommand<User>(ViewUserDetails);
        }

        public ICommand ViewUserDetailsCommand { get; private set; }

        private void ViewUserDetails(User user)
        {
            var parameters = new NavigationParameters();
            parameters.Add("AuthenticatedUser", AuthenticatedUser);
            parameters.Add("ID", user.ID);

            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(userDetailsViewName + parameters, UriKind.Relative));

        }


    }
}
