using DataLayer.Repositories;
using ESD.JC_ReasonMgmt.Views;
using ESD.JC_ReasonMgmt.Services;
using ESD.JC_Infrastructure;
using Prism.Modularity;
using Microsoft.Practices.Unity;
using Prism.Regions;
using Prism.Unity;

namespace ESD.JC_ReasonMgmt
{
    public class ReasonMgmtModule : IModule
    {
        private readonly IUnityContainer container;
        private readonly IRegionManager regionManager;

        public ReasonMgmtModule(IUnityContainer container, IRegionManager regionManager)
        {
            this.container = container;
            this.regionManager = regionManager;
        }

        public void Initialize()
        {
            this.container.RegisterType<IReasonRepository, ReasonRepository>();
            this.container.RegisterType<IReasonServices, ReasonServices>();

            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, () => this.container.Resolve<ReasonMgmtNavigationItemView>());
            this.regionManager.RegisterViewWithRegion(RegionNames.MainContentRegion, () => this.container.Resolve<ReasonMainView>());
        }
    }
}
