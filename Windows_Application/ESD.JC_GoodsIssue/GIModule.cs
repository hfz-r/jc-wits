using Prism.Modularity;
using Microsoft.Practices.Unity;
using Prism.Regions;
using DataLayer.Repositories;
using ESD.JC_Infrastructure;
using ESD.JC_GoodsIssue.Views;
using ESD.JC_GoodsIssue.Services;
using Prism.Unity;

namespace ESD.JC_GoodsIssue
{
    public class GIModule : IModule
    {
        private readonly IUnityContainer container;
        private readonly IRegionManager regionManager;

        public GIModule(IUnityContainer container, IRegionManager regionManager)
        {
            this.container = container;
            this.regionManager = regionManager;
        }

        public void Initialize()
        {
            this.container.RegisterType<IGoodsIssueRepository, GoodsIssueRepository>();
            this.container.RegisterType<IGIServices, GIServices>();

            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, () => this.container.Resolve<GINavigationItemView>());
            this.container.RegisterTypeForNavigation<GIMainView>();
            this.container.RegisterTypeForNavigation<GIMainViewExt>();
        }
    }
}
