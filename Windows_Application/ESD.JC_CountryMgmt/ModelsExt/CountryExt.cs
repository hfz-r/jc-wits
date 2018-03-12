using ESD.JC_CountryMgmt.Utilities;
using DataLayer;

namespace ESD.JC_CountryMgmt.ModelsExt
{
    public class CountryExt : ObservableObject, ISequencedObject
    {
        private int _SequenceNumber;
        public int SequenceNumber
        {
            get { return _SequenceNumber; }
            set
            {
                _SequenceNumber = value;
                base.RaisePropertyChangedEvent("SequenceNumber");
            }
        }

        private long _ID;
        public long ID
        {
            get { return _ID; }
            set
            {
                _ID = value;
                base.RaisePropertyChangedEvent("ID");
            }
        }

        private string _CountryDesc;
        public string CountryDesc
        {
            get { return _CountryDesc; }
            set
            {
                _CountryDesc = value;
                base.RaisePropertyChangedEvent("CountryDesc");
            }
        }

        private System.DateTime _ModifiedOn;
        public System.DateTime ModifiedOn
        {
            get { return _ModifiedOn; }
            set
            {
                _ModifiedOn = value;
                base.RaisePropertyChangedEvent("ModifiedOn");
            }
        }

        private string _ModifiedBy;
        public string ModifiedBy
        {
            get { return _ModifiedBy; }
            set
            {
                _ModifiedBy = value;
                base.RaisePropertyChangedEvent("ModifiedBy");
            }
        }

        #region Constructors

        public CountryExt()
        {
        }

        public CountryExt(long itemID,
                         string itemCountryDesc,
                         System.DateTime itemModifiedOn,
                         string itemModifiedBy)
        {
            _ID = itemID;
            _CountryDesc = itemCountryDesc;
            _ModifiedOn = itemModifiedOn;
            _ModifiedBy = itemModifiedBy;
        }

        public CountryExt(int itemIndex,
                         long itemID,
                         string itemCountryDesc,
                         System.DateTime itemModifiedOn,
                         string itemModifiedBy)
        {
            _SequenceNumber = itemIndex;
            _ID = itemID;
            _CountryDesc = itemCountryDesc;
            _ModifiedOn = itemModifiedOn;
            _ModifiedBy = itemModifiedBy;
        }

        #endregion
    }
}
