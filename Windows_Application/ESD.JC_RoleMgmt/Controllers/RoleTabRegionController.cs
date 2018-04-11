using ESD.JC_Infrastructure;
using ESD.JC_Infrastructure.Events;
using ESD.JC_RoleMgmt.ViewModels;
using ESD.JC_RoleMgmt.Views;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Regions;
using ESD.JC_RoleMgmt.Services;
using DataLayer;
using System.Collections.ObjectModel;
using System.Linq;

namespace ESD.JC_RoleMgmt.Controllers
{
    public class RoleTabRegionController
    {
        private IUnityContainer container;
        private IRegionManager regionManager;
        private IEventAggregator eventAggregator;
        private IRoleServices roleServices;

        public RoleTabRegionController(IUnityContainer container, IRegionManager regionManager, IEventAggregator eventAggregator, IRoleServices roleServices)
        {
            this.container = container;
            this.regionManager = regionManager;
            this.roleServices = roleServices;
            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<RoleSelectedEvent>().Subscribe(InitTabRegion, true);
        }

        private void InitTabRegion(long ID)
        {
            if (ID == 0) return;

            ObservableCollection<User> userCollection = new ObservableCollection<User>();

            var users = roleServices.GetRoleWithAssociated(ID).Select(usr => usr.Users);
            foreach (var obj in users.First())
            {
                userCollection.Add(obj);
            }

            ObservableCollection<ModuleAccessCtrlTransaction> moduleCollection = new ObservableCollection<ModuleAccessCtrlTransaction>();

            var modules = roleServices.GetRoleWithAssociated(ID).Select(mod => mod.ModuleAccessCtrlTransactions);
            foreach (var obj in modules.First())
            {
                moduleCollection.Add(obj);
            }

            IRegion tabRegion = regionManager.Regions[RegionNames.TabRegionRole];
            if (tabRegion == null) return;

            RoleDetailsUsersView userView = tabRegion.GetView("RoleDetailsUsersView") as RoleDetailsUsersView;
            RoleDetailsModuleView moduleView = tabRegion.GetView("RoleDetailsModuleView") as RoleDetailsModuleView;
            if (userView == null && moduleView == null)
            {
                userView = this.container.Resolve<RoleDetailsUsersView>();
                moduleView = this.container.Resolve<RoleDetailsModuleView>();

                tabRegion.Add(userView, "RoleDetailsUsersView");
                tabRegion.Add(moduleView, "RoleDetailsModuleView");
            }

            RoleDetailsUsersViewModel usersVM = userView.DataContext as RoleDetailsUsersViewModel;
            RoleDetailsModuleViewModel moduleVM = moduleView.DataContext as RoleDetailsModuleViewModel;
            if (usersVM != null && moduleVM != null)
            {
                usersVM.userCollection = userCollection;
                moduleVM.modCollection = moduleCollection;
            }
        }
    }
}
