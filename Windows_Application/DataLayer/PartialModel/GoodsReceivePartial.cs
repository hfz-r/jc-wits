//namespace DataLayer
//{
//    public partial class GoodsReceive
//    {
//        public bool IsChecked { get; set; }
//    }
//}

using System.ComponentModel;

namespace DataLayer
{
    public partial class GoodsReceive : INotifyPropertyChanged
    {
        public bool IsChecked { get; set; }

        private bool? _ok2;
        public bool? Ok2
        {
            get { return _ok2; }
            set
            {
                _ok2 = value;
                OnPropertyChanged("Ok2");
            }
        }

        private decimal _quantity2;
        public decimal Quantity2
        {
            get { return _quantity2; }
            set
            {
                _quantity2 = value;
                OnPropertyChanged("Quantity2");
            }
        }

        private decimal? _qtyReceived2;
        public decimal? QtyReceived2
        {
            get { return _qtyReceived2; }
            set
            {
                _qtyReceived2 = value;
                OnPropertyChanged("QtyReceived2");
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged
    }
}

