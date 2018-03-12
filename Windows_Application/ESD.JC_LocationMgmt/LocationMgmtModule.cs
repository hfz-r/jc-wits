using DataLayer.Repositories;
using ESD.JC_LocationMgmt.Views;
using ESD.JC_LocationMgmt.Services;
using ESD.JC_Infrastructure;
using Prism.Modularity;
using Microsoft.Practices.Unity;
using Prism.Regions;
using Prism.Unity;

namespace ESD.JC_LocationMgmt
{
    public class LocationMgmtModule : IModule
    {
        private readonly IUnityContainer container;
        private readonly IRegionManager regionManager;

        public LocationMgmtModule(IUnityContainer container, IRegionManager regionManager)
        {
            this.container = container;
            this.regionManager = regionManager;
        }

        public void Initialize()
        {
            this.container.RegisterType<ILocationRepository, LocationRepository>();
            this.container.RegisterType<ILocationServices, LocationServices>();

            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, () => this.container.Resolve<LocationMgmtNavigationItemView>());
            this.regionManager.RegisterViewWithRegion(RegionNames.MainContentRegion, () => this.container.Resolve<LocationMainView>());
        }
    }
}
