using ESD.JC_Infrastructure;
using ESD.JC_Infrastructure.Events;
using ESD.JC_UserMgmt.ViewModels;
using ESD.JC_UserMgmt.Views;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Regions;
using ESD.JC_UserMgmt.Services;
using DataLayer;
using System.Collections.ObjectModel;

namespace ESD.JC_UserMgmt.Controllers
{
    public class UserTabRegionController
    {
        private IUnityContainer container;
        private IRegionManager regionManager;
        private IEventAggregator eventAggregator;
        private IUserServices userServices;

        public UserTabRegionController(IUnityContainer container, IRegionManager regionManager, IEventAggregator eventAggregator, IUserServices userServices)
        {
            this.container = container;
            this.regionManager = regionManager;
            this.userServices = userServices;
            this.eventAggregator = eventAggregator;
            //this.eventAggregator.GetEvent<UserSelectedEvent>().Subscribe(InitTabRegion, true);
        }

        //private void InitTabRegion(long ID)
        //{
        //    if (ID == 0) return;

        //    ObservableCollection<GRTransaction> grTrnxCollection = new ObservableCollection<GRTransaction>();

        //    var grTrnx = userServices.GetUserGRTrnx(ID);
        //    foreach (var obj in grTrnx)
        //    {
        //        grTrnxCollection.Add(obj);
        //    }

        //    ObservableCollection<GITransaction> giTrnxCollection = new ObservableCollection<GITransaction>();

        //    var giTrnx = userServices.GetUserGITrnx(ID);
        //    foreach (var obj in giTrnx)
        //    {
        //        giTrnxCollection.Add(obj);
        //    }

        //    IRegion tabRegion = regionManager.Regions[RegionNames.TabRegionUser];
        //    if (tabRegion == null) return;

        //    UserDetailsGRTrnxView grTrnxView = tabRegion.GetView("UserDetailsGRTrnxView") as UserDetailsGRTrnxView;
        //    UserDetailsGITrnxView giTrnxView = tabRegion.GetView("UserDetailsGITrnxView") as UserDetailsGITrnxView;
        //    if (grTrnxView == null || giTrnxView == null)
        //    {
        //        grTrnxView = this.container.Resolve<UserDetailsGRTrnxView>();
        //        giTrnxView = this.container.Resolve<UserDetailsGITrnxView>();

        //        tabRegion.Add(grTrnxView, "UserDetailsGRTrnxView");
        //        tabRegion.Add(giTrnxView, "UserDetailsGITrnxView");
        //    }

        //    UserDetailsGRTrnxViewModel grTrnxVM = grTrnxView.DataContext as UserDetailsGRTrnxViewModel;
        //    UserDetailsGITrnxViewModel giTrnxVM = giTrnxView.DataContext as UserDetailsGITrnxViewModel;
        //    if (grTrnxVM != null || giTrnxVM != null)
        //    {
        //        grTrnxVM.grTrnxCollection = grTrnxCollection;
        //        giTrnxVM.giTrnxCollection = giTrnxCollection;
        //    }
        //}
    }
}
