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
    public class FCUTabRegionController
    {
        private IUnityContainer container;
        private IRegionManager regionManager;
        private IEventAggregator eventAggregator;
        private IFCUTransactionServices fcuTransServices;

        public FCUTabRegionController(IUnityContainer container, IRegionManager regionManager, IEventAggregator eventAggregator, IFCUTransactionServices fcuTransServices)
        {
            this.container = container;
            this.regionManager = regionManager;
            this.fcuTransServices = fcuTransServices;
            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<FCUSelectedEvent>().Subscribe(InitTabRegion, true);
        }

        private void InitTabRegion(long FCUID)
        {
            if (FCUID == 0) return;

            ObservableCollection<FCUTransaction> lst = new ObservableCollection<FCUTransaction>();
            foreach (var obj in fcuTransServices.GetFCUTransactionByFCUID(FCUID))
            {
                lst.Add(obj);
            }

            IRegion tabRegion = regionManager.Regions[RegionNames.TabRegionFCU];
            if (tabRegion == null) return;

            FCUDetailsSummaryView summryView = tabRegion.GetView("FCUDetailsSummaryView") as FCUDetailsSummaryView;
            FCUDetailsTransactionView trnxView = tabRegion.GetView("FCUDetailsTransactionView") as FCUDetailsTransactionView;
            if (summryView == null && trnxView == null)
            {
                summryView = this.container.Resolve<FCUDetailsSummaryView>();
                trnxView = this.container.Resolve<FCUDetailsTransactionView>();

                tabRegion.Add(summryView, "FCUDetailsSummaryView");
                tabRegion.Add(trnxView, "FCUDetailsTransactionView");
            }
            tabRegion.Activate(summryView);

            FCUDetailsSummaryViewModel summryVM = summryView.DataContext as FCUDetailsSummaryViewModel;
            FCUDetailsTransactionViewModel trnxVM = trnxView.DataContext as FCUDetailsTransactionViewModel;
            if (summryVM != null )
            {
                summryVM.summryCollection = lst;
                trnxVM.trnxCollection = lst;
            }
        }
    }
}
