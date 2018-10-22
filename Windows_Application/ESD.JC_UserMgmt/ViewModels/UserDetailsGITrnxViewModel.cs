using DataLayer;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Events;
using System.Collections.ObjectModel;
using ESD.JC_UserMgmt.Services;
using Prism.Commands;
using System.Linq;

namespace ESD.JC_UserMgmt.ViewModels
{
    public class UserDetailsGITrnxViewModel : BindableBase
    {
        public string ViewName
        {
            get { return "GI Transaction"; }
        }

        public ObservableCollection<GITransaction> giTrnxCollection { get; set; }

        private ObservableCollection<GITransactionExt> _giTrnxCollectionExt;
        public ObservableCollection<GITransactionExt> giTrnxCollectionExt
        {
            get { return _giTrnxCollectionExt; }
            set
            {
                SetProperty(ref _giTrnxCollectionExt, value);
                RaisePropertyChanged("giTrnxCollectionExt");
            }
        }

        private IRegionManager regionManager;
        private IEventAggregator eventAggregator;
        private IUserServices userServices;

        public UserDetailsGITrnxViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IUserServices userServices)
        {
            this.regionManager = regionManager;
            this.userServices = userServices;
            this.eventAggregator = eventAggregator;

            OnLoadedCommand = new DelegateCommand(OnLoaded);
        }

        public DelegateCommand OnLoadedCommand { get; private set; }

        private void OnLoaded()
        {
            giTrnxCollectionExt = new ObservableCollection<GITransactionExt>();

            foreach (var item in giTrnxCollection)
            {
                giTrnxCollectionExt.Add(new GITransactionExt
                {
                    ID = item.ID,
                    SAPNo = item.GoodsReceive.Material,
                    TransferType = item.TransferType,
                    QuantityOrdered = item.GoodsReceive.Quantity,
                    QuantityReceived = item.Quantity,
                    Ok = item.GoodsReceive.Ok
                });
            }

            RaisePropertyChanged("giTrnxCollectionExt");
        }
    }

    public class GITransactionExt
    {
        public long ID { get; set; }
        public string SAPNo { get; set; }
        public string TransferType { get; set; }
        public decimal QuantityOrdered { get; set; }
        public decimal QuantityReceived { get; set; }
        public bool? Ok { get; set; }
    }
}
