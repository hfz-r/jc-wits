using System.ComponentModel;

namespace DataLayer
{
    public partial class FCU : INotifyPropertyChanged
    {
        public bool IsChecked { get; set; }
        public long IDSeq { get; set; }

        private bool? _shipStatus2;
        public bool? ShipStatus2
        {
            get { return _shipStatus2; }
            set
            {
                _shipStatus2 = value;
                OnPropertyChanged("ShipStatus2");
            }
        }

        private decimal _qty2;
        public decimal Qty2
        {
            get { return _qty2; }
            set
            {
                _qty2 = value;
                OnPropertyChanged("Qty2");
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

