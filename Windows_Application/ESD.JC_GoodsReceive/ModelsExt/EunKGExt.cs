using ESD.JC_GoodsReceive.Utilities;
using System.ComponentModel;
using System.Text;

namespace ESD.JC_GoodsReceive.ModelsExt
{
    public class EunKGExt : ObservableObject, IDataErrorInfo, ISequencedObject
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

        private long _GRID;
        public long GRID
        {
            get { return _GRID; }
            set
            {
                _GRID = value;
                base.RaisePropertyChangedEvent("GRID");
            }
        }

        private string _PO;
        public string PO
        {
            get { return _PO; }
            set
            {
                _PO = value;
                base.RaisePropertyChangedEvent("PO");
            }

        }

        private string _SAPNO;
        public string SAPNO
        {
            get { return _SAPNO; }
            set
            {
                _SAPNO = value;
                base.RaisePropertyChangedEvent("SAPNO");
            }

        }

        private string _EN;
        public string EN
        {
            get { return _EN; }
            set
            {
                _EN = value;
                base.RaisePropertyChangedEvent("EN");
            }

        }

        private string _EUN;
        public string EUN
        {
            get { return _EUN; }
            set
            {
                _EUN = value;
                base.RaisePropertyChangedEvent("EUN");
            }

        }

        private decimal _Qty;
        public decimal Qty
        {
            get { return _Qty; }
            set
            {
                _Qty = value;
                base.RaisePropertyChangedEvent("Qty");
            }
        }

        private string _BIN;
        public string BIN
        {
            get { return _BIN; }
            set
            {
                _BIN = value;
                base.RaisePropertyChangedEvent("BIN");
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

        private bool _IsChecked;
        public bool IsChecked
        {
            get { return _IsChecked; }
            set
            {
                _IsChecked = value;
                base.RaisePropertyChangedEvent("IsChecked");
            }
        }

        #region Constructors

        public EunKGExt()
        {
        }

        public EunKGExt(long id, long grid, string po, string sapno, string endesc, decimal qy, string bin, System.DateTime mo, string mb)
        {
            _ID = id;
            _GRID = grid;
            _PO = po;
            _SAPNO = sapno;
            _EN = endesc;
            _Qty = qy;
            _BIN = bin;
            _ModifiedOn = mo;
            _ModifiedBy = mb;
        }

        public EunKGExt(int idx , long id, long grid, string po, string sapno, string endesc, decimal qy, string bin, System.DateTime mo, string mb)
        {
            _SequenceNumber = idx;
            _ID = id;
            _GRID = grid;
            _PO = po;
            _SAPNO = sapno;
            _EN = endesc;
            _Qty = qy;
            _BIN = bin;
            _ModifiedOn = mo;
            _ModifiedBy = mb;
        }

        #endregion

        #region IDataErrorInfo Members

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();

                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor prop in props)
                {
                    string propertyError = this[prop.Name];
                    if (propertyError != string.Empty)
                    {
                        error.Append((error.Length != 0 ? ", " : "") + propertyError);
                    }
                }
               
                return error.ToString();
            }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == "PO")
                {
                    if (PO == null || PO == string.Empty)
                        return "PO cannot be null or empty";
                }

                if (columnName == "Qty")
                {
                    if (Qty < 0)
                        return "Quantity must be positive";
                }

                return "";
            }
        }

        #endregion
    }
}
