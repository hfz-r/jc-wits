using Prism.Modularity;
using Microsoft.Practices.Unity;
using Prism.Regions;
using DataLayer.Repositories;
using ESD.JC_Infrastructure;
using ESD.JC_GoodsIssue.Views;
using ESD.JC_GoodsIssue.Services;
using ESD.JC_GoodsIssue.Controllers;
using Prism.Unity;

namespace ESD.JC_GoodsIssue
{
    public class GIModule : IModule
    {
        private readonly IUnityContainer container;
        private readonly IRegionManager regionManager;

        private GITabRegionController TabRegionController;

        public GIModule(IUnityContainer container, IRegionManager regionManager)
        {
            this.container = container;
            this.regionManager = regionManager;
        }

        public void Initialize()
        {
            this.container.RegisterType<IGoodsIssueRepository, GoodsIssueRepository>();
            this.container.RegisterType<IGIServices, GIServices>();
            this.container.RegisterType<IGITransactionRepository, GITransactionRepository>();
            this.container.RegisterType<IGITransactionServices, GITransactionServices>();
            this.container.RegisterType<IGITimerSevices, GITimerServices>();

            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, () => this.container.Resolve<GINavigationItemView>());
            this.container.RegisterTypeForNavigation<GIMainView>();
            this.container.RegisterTypeForNavigation<GIDetailsView>();

            this.TabRegionController = this.container.Resolve<GITabRegionController>();
        }
    }
}
