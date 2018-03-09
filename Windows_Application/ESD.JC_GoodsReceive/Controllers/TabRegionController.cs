using ESD.JC_Infrastructure;
using ESD.JC_Infrastructure.Events;
using ESD.JC_GoodsReceive.ViewModels;
using ESD.JC_GoodsReceive.Views;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Regions;
using ESD.JC_GoodsReceive.Services;
using DataLayer;
using System.Collections.ObjectModel;

namespace ESD.JC_GoodsReceive.Controllers
{
    public class TabRegionController
    {
        private IUnityContainer container;
        private IRegionManager regionManager;
        private IEventAggregator eventAggregator;
        private IGRTransactionServices grTransServices;

        public TabRegionController(IUnityContainer container, IRegionManager regionManager, IEventAggregator eventAggregator, IGRTransactionServices grTransServices)
        {
            this.container = container;
            this.regionManager = regionManager;
            this.grTransServices = grTransServices;
            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<GRUserSelectedEvent>().Subscribe(InitTabRegion, true);
        }

        private void InitTabRegion(long GRID)
        {
            if (GRID == 0) return;

            ObservableCollection<GRTransaction> lst = new ObservableCollection<GRTransaction>();
            foreach (var obj in grTransServices.GetGRTransactionByGRID(GRID))
            {
                lst.Add(obj);
            }

            IRegion tabRegion = regionManager.Regions[RegionNames.TabRegionGR];
            if (tabRegion == null) return;

            GRDetailsSummaryView summryView = tabRegion.GetView("GRDetailsSummaryView") as GRDetailsSummaryView;
            GRDetailsTransactionView trnxView = tabRegion.GetView("GRDetailsTransactionView") as GRDetailsTransactionView;
            if (summryView == null && trnxView == null)
            {
                summryView = this.container.Resolve<GRDetailsSummaryView>();
                trnxView = this.container.Resolve<GRDetailsTransactionView>();

                tabRegion.Add(summryView, "GRDetailsSummaryView");
                tabRegion.Add(trnxView, "GRDetailsTransactionView");
            }
            tabRegion.Activate(summryView);

            GRDetailsSummaryViewModel summryVM = summryView.DataContext as GRDetailsSummaryViewModel;
            GRDetailsTransactionViewModel trnxVM = trnxView.DataContext as GRDetailsTransactionViewModel;
            if (summryVM != null && trnxVM != null)
            {
                summryVM.summryCollection = lst;
                trnxVM.trnxCollection = lst;
            }
        }
    }
}
