using Prism.Modularity;
using DataLayer.Repositories;
using ESD.JC_Infrastructure;
using ESD.JC_GoodsReceive.Views;
using Microsoft.Practices.Unity;
using Prism.Regions;
using ESD.JC_GoodsReceive.Services;
using Prism.Unity;

namespace ESD.JC_GoodsReceive
{
    public class GRModule : IModule
    {
        private readonly IUnityContainer container;
        private readonly IRegionManager regionManager;

        public GRModule(IUnityContainer container, IRegionManager regionManager)
        {
            this.container = container;
            this.regionManager = regionManager;
        }

        public void Initialize()
        {
            this.container.RegisterType<IGoodsReceiveRepository, GoodsReceiveRepository>();
            this.container.RegisterType<IGRServices, GRServices>();

            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, () => this.container.Resolve<GRNavigationItemView>());
            //this.regionManager.RegisterViewWithRegion(RegionNames.MainContentRegion, () => this.container.Resolve<GRMainView>());
            this.container.RegisterTypeForNavigation<GRMainView>();
            this.container.RegisterTypeForNavigation<GRDetailsView>();
        }
    }
}
