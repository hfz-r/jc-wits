using DataLayer;
using ESD.JC_FinishGoods.Services;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace ESD.JC_FinishGoods.ViewModels
{
    public class FCUDetailsTransactionViewModel : BindableBase
    {
        public string ViewName
        {
            get { return "Transaction Details"; }
        }

        public ObservableCollection<FCUTransaction> trnxCollection { get; set; }

        private ObservableCollection<FCUTransactionExt> _fcuTrnxCollectionExt;
        public ObservableCollection<FCUTransactionExt> FCUtrnxCollectionExt
        {
            get { return _fcuTrnxCollectionExt; }
            set { SetProperty(ref _fcuTrnxCollectionExt, value); }
        }

        private IFCUTransactionServices FCUTransactionServices;

        public FCUDetailsTransactionViewModel(IFCUTransactionServices FCUTransactionServices)
        {
            this.FCUTransactionServices = FCUTransactionServices;

            OnLoadedCommand = new DelegateCommand(OnLoaded);
        }

        public DelegateCommand OnLoadedCommand { get; private set; }

        private void OnLoaded()
        {
            FCUtrnxCollectionExt = new ObservableCollection<FCUTransactionExt>();

            foreach (var item in trnxCollection)
            {
                FCUTransactionExt ext = new FCUTransactionExt();
                ext.FT = item;

                if (item.CountryID.HasValue)
                    ext.ShipTo = item.Country.CountryDesc;
                else if (item.LocationID.HasValue)
                {
                    var loc = FCUTransactionServices.GetLocationFromFCUTransaction(item.LocationID.Value);
                    ext.ShipTo = loc.LocationDesc + " (Location)";
                }

                FCUtrnxCollectionExt.Add(ext);
            }

            RaisePropertyChanged("FCUTransactionExt");
        }
    }

    public class FCUTransactionExt
    {
        public FCUTransaction FT { get; set; }
        public string ShipTo { get; set; }
    }

}
