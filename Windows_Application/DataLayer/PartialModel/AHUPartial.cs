using System.ComponentModel;

namespace DataLayer
{
    public partial class AHU : INotifyPropertyChanged
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

        private int? _section2;
        public int? Section2
        {
            get { return _section2; }
            set
            {
                _section2 = value;
                OnPropertyChanged("Section2");
            }
        }

        private int? _sectionReceived2;
        public int? SectionReceived2
        {
            get { return _sectionReceived2; }
            set
            {
                _sectionReceived2 = value;
                OnPropertyChanged("SectionReceived2");
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