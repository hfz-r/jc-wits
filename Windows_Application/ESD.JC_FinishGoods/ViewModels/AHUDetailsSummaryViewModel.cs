using DataLayer;
using ESD.JC_FinishGoods.Services;
using Prism.Mvvm;
using Prism.Regions;
using Microsoft.Practices.Unity;
using System.Linq;
using System.Collections.ObjectModel;

namespace ESD.JC_FinishGoods.ViewModels
{
    public class AHUDetailsSummaryViewModel : BindableBase
    {
        public string ViewName
        {
            get { return "Transaction Summary"; }
        }

        private ObservableCollection<AHUTransaction> _summryCollection;
        public ObservableCollection<AHUTransaction> summryCollection
        {
            get { return _summryCollection; }
            set
            {
                SetProperty(ref _summryCollection, value);
                if (summryCollection.Count() > 0)
                {
                    var ahuObj = summryCollection.Select(x => x.AHU).FirstOrDefault();
                    RemainingSect = ahuObj.Section.GetValueOrDefault() - ahuObj.SectionReceived.GetValueOrDefault();

                    CountCountries = summryCollection.GroupBy(c => c.Country.ID).Count();
                }

                RaisePropertyChanged("RemainingSect");
                RaisePropertyChanged("CountCountries");
            }
        }

        private int _RemainingSect = 0;
        public int RemainingSect
        {
            get { return _RemainingSect; }
            set { SetProperty(ref _RemainingSect, value); }
        }

        private int _CountCountries;
        public int CountCountries
        {
            get { return _CountCountries; }
            set { SetProperty(ref _CountCountries, value); }
        }

        private IUnityContainer Container;
        private IRegionManager RegionManager;
        private IAHUTransactionServices AHUTransactionServices;

        public AHUDetailsSummaryViewModel(IUnityContainer _Container, IRegionManager _RegionManager, IAHUTransactionServices _AHUTransactionServices)
        {
            Container = _Container;
            RegionManager = _RegionManager;
            AHUTransactionServices = _AHUTransactionServices;
        }
    }
}
