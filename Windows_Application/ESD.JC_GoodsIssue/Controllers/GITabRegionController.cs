using ESD.JC_Infrastructure;
using ESD.JC_Infrastructure.Events;
using ESD.JC_GoodsIssue.ViewModels;
using ESD.JC_GoodsIssue.Views;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Regions;
using ESD.JC_GoodsIssue.Services;
using DataLayer;
using System.Collections.ObjectModel;

namespace ESD.JC_GoodsIssue.Controllers
{
    public class GITabRegionController
    {
        private IUnityContainer container;
        private IRegionManager regionManager;
        private IEventAggregator eventAggregator;
        private IGITransactionServices giTransServices;

        public GITabRegionController(IUnityContainer container, IRegionManager regionManager, IEventAggregator eventAggregator, IGITransactionServices giTransServices)
        {
            this.container = container;
            this.regionManager = regionManager;
            this.giTransServices = giTransServices;
            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<GIUserSelectedEvent>().Subscribe(InitTabRegion, true);
        }

        private void InitTabRegion(long ID)
        {
            //if (ID == 0) return;

            //ObservableCollection<GITransaction> lst = new ObservableCollection<GITransaction>();
            //foreach (var obj in giTransServices.GetGITransaction(ID.ToString()))
            //{
            //    lst.Add(obj);
            //}

            //IRegion tabRegion = regionManager.Regions[RegionNames.TabRegionGI];
            //if (tabRegion == null) return;

            //GIDetailsSummaryView summryView = tabRegion.GetView("GIDetailsSummaryView") as GIDetailsSummaryView;
            //GIDetailsTransactionView trnxView = tabRegion.GetView("GIDetailsTransactionView") as GIDetailsTransactionView;
            //if (summryView == null && trnxView == null)
            //{
            //    summryView = this.container.Resolve<GIDetailsSummaryView>();
            //    trnxView = this.container.Resolve<GIDetailsTransactionView>();

            //    tabRegion.Add(summryView, "GIDetailsSummaryView");
            //    tabRegion.Add(trnxView, "GIDetailsTransactionView");
            //}
            //tabRegion.Activate(summryView);

            //GIDetailsSummaryViewModel summryVM = summryView.DataContext as GIDetailsSummaryViewModel;
            //GIDetailsTransactionViewModel trnxVM = trnxView.DataContext as GIDetailsTransactionViewModel;
            //if (summryVM != null && trnxVM != null)
            //{
            //    summryVM.summryCollection = lst;
            //    trnxVM.trnxCollection = lst;
            //}
        }
    }
}
