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
    public class UserDetailsGRTrnxViewModel : BindableBase
    {
        public string ViewName
        {
            get { return "GR Transaction"; }
        }

        public ObservableCollection<GRTransaction> grTrnxCollection { get; set; }

        private ObservableCollection<GRTransactionExt> _grTrnxCollectionExt;
        public ObservableCollection<GRTransactionExt> grTrnxCollectionExt
        {
            get { return _grTrnxCollectionExt; }
            set
            {
                SetProperty(ref _grTrnxCollectionExt, value);
                RaisePropertyChanged("grTrnxCollectionExt");
            }
        }

        private IRegionManager regionManager;
        private IEventAggregator eventAggregator;
        private IUserServices userServices;

        public UserDetailsGRTrnxViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IUserServices userServices)
        {
            this.regionManager = regionManager;
            this.userServices = userServices;
            this.eventAggregator = eventAggregator;

            OnLoadedCommand = new DelegateCommand(OnLoaded);
        }

        public DelegateCommand OnLoadedCommand { get; private set; }

        private void OnLoaded()
        {
            grTrnxCollectionExt = new ObservableCollection<GRTransactionExt>();

            foreach (var item in grTrnxCollection)
            {
                grTrnxCollectionExt.Add(new GRTransactionExt
                {
                    ID = item.ID,
                    SAPNo = item.GoodsReceive.Material,
                    QuantityOrdered = item.GoodsReceive.Quantity,
                    QuantityReceived = item.Quantity,
                    Reason = item.Reason.ReasonDesc,
                    Ok = item.GoodsReceive.Ok
                });
            }

            RaisePropertyChanged("grTrnxCollectionExt");
        }
    }

    public class GRTransactionExt
    {
        public long ID { get; set; }
        public string SAPNo { get; set; }
        public decimal QuantityOrdered { get; set; }
        public decimal QuantityReceived { get; set; }
        public string Reason { get; set; }
        public bool? Ok { get; set; }
    }
}
