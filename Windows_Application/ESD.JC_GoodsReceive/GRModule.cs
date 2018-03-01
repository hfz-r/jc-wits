using Prism.Modularity;
using Microsoft.Practices.Unity;
using Prism.Regions;
using DataLayer.Repositories;
using ESD.JC_GoodsReceive.Services;
using ESD.JC_Infrastructure;
using ESD.JC_GoodsReceive.Views;
using Prism.Unity;
using ESD.JC_GoodsReceive.Controllers;

namespace ESD.JC_GoodsReceive
{
    public class GRModule : IModule
    {
        private readonly IUnityContainer container;
        private readonly IRegionManager regionManager;

        private TabRegionController TabRegionController;

        public GRModule(IUnityContainer container, IRegionManager regionManager)
        {
            this.container = container;
            this.regionManager = regionManager;
        }

        public void Initialize()
        {
            this.container.RegisterType<IGoodsReceiveRepository, GoodsReceiveRepository>();
            this.container.RegisterType<IGRServices, GRServices>();
            this.container.RegisterType<IGRTransactionRepository, GRTransactionRepository>();
            this.container.RegisterType<IGRTransactionServices, GRTransactionServices>();
            this.container.RegisterType<IEunKGRepository, EunKGRepository>();
            this.container.RegisterType<IEunKGServices, EunKGServices>();

            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, () => this.container.Resolve<GRNavigationItemView>());
            this.container.RegisterTypeForNavigation<GRMainView>();
            this.container.RegisterTypeForNavigation<GRDetailsView>();

            this.TabRegionController = this.container.Resolve<TabRegionController>();
        }
    }
}
