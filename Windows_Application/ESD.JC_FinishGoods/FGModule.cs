using Prism.Modularity;
using Microsoft.Practices.Unity;
using Prism.Regions;
using Prism.Unity;
using DataLayer.Repositories;
using ESD.JC_Infrastructure;
using ESD.JC_FinishGoods.Views;
using ESD.JC_FinishGoods.Services;
using ESD.JC_Infrastructure.Controls;
using ESD.JC_FinishGoods.Controllers;

namespace ESD.JC_FinishGoods
{
    public class FGModule : IModule
    {
        private readonly IUnityContainer container;
        private readonly IRegionManager regionManager;

        private TabRegionController TabRegionController;
        private FCUTabRegionController FCUTabRegionController;
        private AHUTabRegionController AHUTabRegionController;

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
            this.container.RegisterType<IFCUTransactionRepository, FCUTransactionRepository>();
            this.container.RegisterType<IFCUTransactionServices, FCUTransactionServices>();
            this.container.RegisterType<IAHURepository, AHURepository>();
            this.container.RegisterType<IAHUServices, AHUServices>();
            this.container.RegisterType<IAHUTransactionRepository, AHUTransactionRepository>();
            this.container.RegisterType<IAHUTransactionServices, AHUTransactionServices>();

            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, () => this.container.Resolve<FGNavigationItemView>());
            this.container.RegisterTypeForNavigation<FGMainView>();
            this.container.RegisterTypeForNavigation<FGfcuDetailsView>();
            this.container.RegisterTypeForNavigation<FGahuDetailsView>();

            this.TabRegionController = this.container.Resolve<TabRegionController>();
            this.FCUTabRegionController = this.container.Resolve<FCUTabRegionController>();
            this.AHUTabRegionController = this.container.Resolve<AHUTabRegionController>();
        }
    }
}
