using DataLayer.Repositories;
using ESD.JC_CountryMgmt.Views;
using ESD.JC_CountryMgmt.Services;
using ESD.JC_Infrastructure;
using Prism.Modularity;
using Microsoft.Practices.Unity;
using Prism.Regions;
using Prism.Unity;

namespace ESD.JC_CountryMgmt
{
    public class CountryMgmtModule : IModule
    {
        private readonly IUnityContainer container;
        private readonly IRegionManager regionManager;

        public CountryMgmtModule(IUnityContainer container, IRegionManager regionManager)
        {
            this.container = container;
            this.regionManager = regionManager;
        }

        public void Initialize()
        {
            this.container.RegisterType<ICountryRepository, CountryRepository>();
            this.container.RegisterType<ICountryServices, CountryServices>();

            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, () => this.container.Resolve<CountryMgmtNavigationItemView>());
            this.regionManager.RegisterViewWithRegion(RegionNames.MainContentRegion, () => this.container.Resolve<CountryMainView>());
        }
    }
}
