using Prism.Modularity;
using Microsoft.Practices.Unity;
using Prism.Regions;
using Prism.Unity;
using DataLayer.Repositories;
using ESD.JC_Infrastructure;
using ESD.JC_FinishGoods.Views;
using ESD.JC_FinishGoods.Services;
using ESD.JC_FinishGoods.ViewModels;
using ESD.JC_Infrastructure.Controls;
using ESD.JC_FinishGoods.Controllers;

namespace ESD.JC_FinishGoods
{
    public class FGModule : IModule
    {
        private readonly IUnityContainer container;
        private readonly IRegionManager regionManager;

        private TabRegionController TabRegionController;

        public FGModule(IUnityContainer container, IRegionManager regionManager)
        {
            this.container = container;
            this.regionManager = regionManager;
        }

        public void Initialize()
        {
            this.container.RegisterType<ICompositeCommands, CompositeCommands>(new ContainerControlledLifetimeManager());
            this.container.RegisterType<IFCURepository, FCURepository>();
            this.container.RegisterType<IFCUServices, FCUServices>();
            this.container.RegisterType<IAHURepository, AHURepository>();
            this.container.RegisterType<IAHUServices, AHUServices>();

            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, () => this.container.Resolve<FGNavigationItemView>());
            this.container.RegisterTypeForNavigation<FGMainView>();

            this.TabRegionController = this.container.Resolve<TabRegionController>();
        }
    }
}
