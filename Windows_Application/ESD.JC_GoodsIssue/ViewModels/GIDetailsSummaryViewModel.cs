using DataLayer;
using ESD.JC_GoodsIssue.Services;
using Prism.Mvvm;
using Prism.Regions;
using Microsoft.Practices.Unity;
using System.Linq;
using System.Collections.ObjectModel;

namespace ESD.JC_GoodsIssue.ViewModels
{
    public class GIDetailsSummaryViewModel : BindableBase
    {
        public string ViewName
        {
            get { return "Transaction Summary"; }
        }

        private ObservableCollection<GITransaction> _summryCollection;
        public ObservableCollection<GITransaction> summryCollection
        {
            get { return _summryCollection; }
            set
            {
                SetProperty(ref _summryCollection, value);
                if (summryCollection.Count() > 0)
                {
                    TotalFrom = summryCollection.GroupBy(c => c.LocationFromID).Count();
                    TotalTo = summryCollection.GroupBy(c => c.LocationToID).Count();
                }

                RaisePropertyChanged("TotalFrom");
                RaisePropertyChanged("TotalTo");
            }
        }

        private int _TotalTo;
        public int TotalTo
        {
            get { return _TotalTo; }
            set { SetProperty(ref _TotalTo, value); }
        }

        private int _TotalFrom;
        public int TotalFrom
        {
            get { return _TotalFrom; }
            set { SetProperty(ref _TotalFrom, value); }
        }

        private IUnityContainer Container;
        private IRegionManager RegionManager;
        private IGITransactionServices GITransactionServices;

        public GIDetailsSummaryViewModel(IUnityContainer _Container, IRegionManager _RegionManager, IGITransactionServices _GITransactionServices)
        {
            Container = _Container;
            RegionManager = _RegionManager;
            GITransactionServices = _GITransactionServices;
        }
    }
}
