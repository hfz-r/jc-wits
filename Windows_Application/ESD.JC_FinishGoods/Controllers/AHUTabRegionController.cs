using ESD.JC_Infrastructure;
using ESD.JC_Infrastructure.Events;
using ESD.JC_FinishGoods.ViewModels;
using ESD.JC_FinishGoods.Views;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Regions;
using ESD.JC_FinishGoods.Services;
using DataLayer;
using System.Collections.ObjectModel;

namespace ESD.JC_FinishGoods.Controllers
{
    public class AHUTabRegionController
    {
        private IUnityContainer container;
        private IRegionManager regionManager;
        private IEventAggregator eventAggregator;
        private IAHUTransactionServices ahuTransServices;

        public AHUTabRegionController(IUnityContainer container, IRegionManager regionManager, IEventAggregator eventAggregator, IAHUTransactionServices ahuTransServices)
        {
            this.container = container;
            this.regionManager = regionManager;
            this.ahuTransServices = ahuTransServices;
            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<AHUSelectedEvent>().Subscribe(InitTabRegion, true);
        }

        private void InitTabRegion(long AHUID)
        {
            if (AHUID == 0) return;

            AHU objSummry = new AHU();
            objSummry = ahuTransServices.GetAHUDetails(AHUID);

            ObservableCollection<AHUTransaction> lstTrnx = new ObservableCollection<AHUTransaction>();
            foreach (var obj in ahuTransServices.GetAHUTransactionByAHUID(AHUID))
            {
                lstTrnx.Add(obj);
            }

            IRegion tabRegion = regionManager.Regions[RegionNames.TabRegionAHU];
            if (tabRegion == null) return;

            AHUDetailsSummaryView summryView = tabRegion.GetView("AHUDetailsSummaryView") as AHUDetailsSummaryView;
            AHUDetailsTransactionView trnxView = tabRegion.GetView("AHUDetailsTransactionView") as AHUDetailsTransactionView;
            if (summryView == null && trnxView == null)
            {
                summryView = this.container.Resolve<AHUDetailsSummaryView>();
                trnxView = this.container.Resolve<AHUDetailsTransactionView>();

                tabRegion.Add(summryView, "AHUDetailsSummaryView");
                tabRegion.Add(trnxView, "AHUDetailsTransactionView");
            }
            tabRegion.Activate(summryView);

            AHUDetailsSummaryViewModel summryVM = summryView.DataContext as AHUDetailsSummaryViewModel;
            AHUDetailsTransactionViewModel trnxVM = trnxView.DataContext as AHUDetailsTransactionViewModel;
            if (summryVM != null && trnxVM != null)
            {
                summryVM.summryObj = objSummry;
                trnxVM.trnxCollection = lstTrnx;
            }
        }
    }
}
