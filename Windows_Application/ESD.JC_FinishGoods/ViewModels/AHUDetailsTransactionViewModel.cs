using DataLayer;
using ESD.JC_FinishGoods.Services;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace ESD.JC_FinishGoods.ViewModels
{
    public class AHUDetailsTransactionViewModel : BindableBase
    {
        public string ViewName
        {
            get { return "Transaction Details"; }
        }

        public ObservableCollection<AHUTransaction> trnxCollection { get; set; }

        private ObservableCollection<AHUTransactionExt> _ahuTrnxCollectionExt;
        public ObservableCollection<AHUTransactionExt> AHUtrnxCollectionExt
        {
            get { return _ahuTrnxCollectionExt; }
            set { SetProperty(ref _ahuTrnxCollectionExt, value); }
        }

        private IAHUTransactionServices AHUTransactionServices;

        public AHUDetailsTransactionViewModel(IAHUTransactionServices AHUTransactionServices)
        {
            this.AHUTransactionServices = AHUTransactionServices;

            OnLoadedCommand = new DelegateCommand(OnLoaded);
        }

        public DelegateCommand OnLoadedCommand { get; private set; }

        private void OnLoaded()
        {
            AHUtrnxCollectionExt = new ObservableCollection<AHUTransactionExt>();

            foreach (var item in trnxCollection)
            {
                AHUTransactionExt ext = new AHUTransactionExt();
                ext.AT = item;

                if (item.CountryID.HasValue)
                    ext.ShipTo = item.Country.CountryDesc;
                else if (item.LocationID.HasValue)
                {
                    var loc = AHUTransactionServices.GetLocationFromAHUTransaction(item.LocationID.Value);
                    ext.ShipTo = loc.LocationDesc + " (Location)";
                }

                AHUtrnxCollectionExt.Add(ext);
            }

            RaisePropertyChanged("AHUTransactionExt");
        }
    }

    public class AHUTransactionExt
    {
        public AHUTransaction AT { get; set; }
        public string ShipTo { get; set; }
    }
}
