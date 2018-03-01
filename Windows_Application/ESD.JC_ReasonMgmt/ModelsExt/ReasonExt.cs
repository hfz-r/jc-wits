using ESD.JC_ReasonMgmt.Utilities;
using DataLayer;

namespace ESD.JC_ReasonMgmt.ModelsExt
{
    public class ReasonExt : ObservableObject, ISequencedObject
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

        private string _ReasonDesc;
        public string ReasonDesc
        {
            get { return _ReasonDesc; }
            set
            {
                _ReasonDesc = value;
                base.RaisePropertyChangedEvent("ReasonDesc");
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

        public ReasonExt()
        {
        }

        public ReasonExt(long itemID, 
                         string itemReasonDesc, 
                         System.DateTime itemModifiedOn,
                         string itemModifiedBy)
        {
            _ID = itemID;
            _ReasonDesc = itemReasonDesc;
            _ModifiedOn = itemModifiedOn;
            _ModifiedBy = itemModifiedBy;
        }

        public ReasonExt(int itemIndex,
                         long itemID,
                         string itemReasonDesc,
                         System.DateTime itemModifiedOn,
                         string itemModifiedBy)
        {
            _SequenceNumber = itemIndex;
            _ID = itemID;
            _ReasonDesc = itemReasonDesc;
            _ModifiedOn = itemModifiedOn;
            _ModifiedBy = itemModifiedBy;
        }

        #endregion
    }
}
