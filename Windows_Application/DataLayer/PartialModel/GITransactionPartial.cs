using System.ComponentModel;

namespace DataLayer
{
    public partial class GITransaction : INotifyPropertyChanged
    {
        public bool IsChecked { get; set; }
        

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged
    }
}

