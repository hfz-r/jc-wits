using ESD.JC_LocationMgmt.Utilities;
using DataLayer;

namespace ESD.JC_LocationMgmt.ModelsExt
{
    public class LocationExt : ObservableObject, ISequencedObject
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

        private string _LocationDesc;
        public string LocationDesc
        {
            get { return _LocationDesc; }
            set
            {
                _LocationDesc = value;
                base.RaisePropertyChangedEvent("LocationDesc");
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

        public LocationExt()
        {
        }

        public LocationExt(long itemID,
                         string itemLocationDesc,
                         System.DateTime itemModifiedOn,
                         string itemModifiedBy)
        {
            _ID = itemID;
            _LocationDesc = itemLocationDesc;
            _ModifiedOn = itemModifiedOn;
            _ModifiedBy = itemModifiedBy;
        }

        public LocationExt(int itemIndex,
                         long itemID,
                         string itemLocationDesc,
                         System.DateTime itemModifiedOn,
                         string itemModifiedBy)
        {
            _SequenceNumber = itemIndex;
            _ID = itemID;
            _LocationDesc = itemLocationDesc;
            _ModifiedOn = itemModifiedOn;
            _ModifiedBy = itemModifiedBy;
        }

        #endregion
    }
}
