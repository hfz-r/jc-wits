using DataLayer.Repositories;
using ESD.JC_Infrastructure;
using ESD.JC_UserMgmt.Services;
using ESD.JC_UserMgmt.Views;
using ESD.JC_UserMgmt.Controllers;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;

namespace ESD.JC_UserMgmt
{
    public class UserMgmtModule : IModule
    {
        private readonly IUnityContainer container;
        private readonly IRegionManager regionManager;

        private UserTabRegionController TabRegionController;

        public UserMgmtModule(IUnityContainer container, IRegionManager regionManager)
        {
            this.container = container;
            this.regionManager = regionManager;
        }

        public void Initialize()
        {
            this.container.RegisterType<IUserRepository, UserRepository>();
            this.container.RegisterType<IGRTransactionRepository, GRTransactionRepository>();
            this.container.RegisterType<IGITransactionRepository, GITransactionRepository>();
            this.container.RegisterType<IUserServices, UserServices>();

            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, () => this.container.Resolve<UserMgmtNavigationItemView>());
            this.regionManager.RegisterViewWithRegion(RegionNames.MainContentRegion, () => this.container.Resolve<UserMainView>());
            this.container.RegisterTypeForNavigation<UserDetailsView>();
            //this.container.RegisterTypeForNavigation<UserOperationView>();

            this.TabRegionController = this.container.Resolve<UserTabRegionController>();
        }
    }
}
