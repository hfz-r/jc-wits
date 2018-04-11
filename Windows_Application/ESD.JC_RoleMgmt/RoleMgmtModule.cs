using DataLayer.Repositories;
using ESD.JC_RoleMgmt.Services;
using ESD.JC_RoleMgmt.Views;
using ESD.JC_RoleMgmt.Controllers;
using ESD.JC_Infrastructure;
using Prism.Modularity;
using Microsoft.Practices.Unity;
using Prism.Regions;
using Prism.Unity;

namespace ESD.JC_RoleMgmt
{
    public class RoleMgmtModule : IModule
    {
        private readonly IUnityContainer container;
        private readonly IRegionManager regionManager;

        private RoleTabRegionController TabRegionController;

        public RoleMgmtModule(IUnityContainer container, IRegionManager regionManager)
        {
            this.container = container;
            this.regionManager = regionManager;
        }

        public void Initialize()
        {
            this.container.RegisterType<IRoleRepository, RoleRepository>();
            this.container.RegisterType<IRoleServices, RoleServices>();
            this.container.RegisterType<IModuleAccessCtrlRepository, ModuleAccessCtrlRepository>();
            this.container.RegisterType<IModuleAccessCtrlServices, ModuleAccessCtrlServices>();
            this.container.RegisterType<IModuleAccessCtrlTransactionRepository, ModuleAccessCtrlTransactionRepository>();
            this.container.RegisterType<IModuleAccessCtrlTransactionServices, ModuleAccessCtrlTransactionServices>();

            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, () => this.container.Resolve<RoleMgmtNavigationItemView>());
            //this.regionManager.RegisterViewWithRegion(RegionNames.MainContentRegion, () => this.container.Resolve<RoleMainView>());
            this.container.RegisterTypeForNavigation<RoleMainView>();
            this.container.RegisterTypeForNavigation<RoleDetailsView>();
            this.container.RegisterTypeForNavigation<RoleOperationView>();

            //regionManager.RegisterViewWithRegion(RegionNames.TabRegionRole, () => this.container.Resolve<RoleDetailsUsersView>());
            this.TabRegionController = this.container.Resolve<RoleTabRegionController>();
        }
    }
}
