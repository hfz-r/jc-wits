using DataLayer.Repositories;
using ESD.JC_LabelPrinting.Views;
using ESD.JC_Infrastructure;
using Prism.Modularity;
using Microsoft.Practices.Unity;
using Prism.Regions;
using Prism.Unity;

namespace ESD.JC_LabelPrinting
{
    public class LabelPrintingModule : IModule
    {
        private readonly IUnityContainer container;
        private readonly IRegionManager regionManager;

        public LabelPrintingModule(IUnityContainer container, IRegionManager regionManager)
        {
            this.container = container;
            this.regionManager = regionManager;
        }

        public void Initialize()
        {
            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, () => this.container.Resolve<LabelPrintNavigationItemView>());
            this.container.RegisterTypeForNavigation<LabelPrintMainView>();
        }
    }
}
