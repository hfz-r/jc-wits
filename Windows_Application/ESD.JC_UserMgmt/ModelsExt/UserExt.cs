using ESD.JC_UserMgmt.Utilities;

namespace ESD.JC_UserMgmt.ModelsExt
{
    public class UserExt : ObservableObject, ISequencedObject
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

        private string _Username;
        public string Username
        {
            get { return _Username; }
            set
            {
                _Username = value;
                base.RaisePropertyChangedEvent("Username");
            }
        }

        private string _Password;
        public string Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
                base.RaisePropertyChangedEvent("Password");
            }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                base.RaisePropertyChangedEvent("Name");
            }
        }

        private long? _RoleID;
        public long? RoleID
        {
            get { return _RoleID; }
            set
            {
                _RoleID = value;
                base.RaisePropertyChangedEvent("RoleID");
            }
        }

        private string _Email;
        public string Email
        {
            get { return _Email; }
            set
            {
                _Email = value;
                base.RaisePropertyChangedEvent("Email");
            }
        }

        private string _Address;
        public string Address
        {
            get { return _Address; }
            set
            {
                _Address = value;
                base.RaisePropertyChangedEvent("Address");
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

        private bool _IsEditable;
        public bool IsEditable
        {
            get { return _IsEditable; }
            set
            {
                _IsEditable = value;
                base.RaisePropertyChangedEvent("IsEditable");
            }
        }

        #region Ctor

        public UserExt() { }

        public UserExt(long id, string username, string name, string password, long roleid, System.DateTime modifiedon, string modifiedby, bool iseditable)
        {
            _ID = id;
            _Username = username;
            _Password = 
            _Name = name;
            _RoleID = roleid;
            _ModifiedOn = modifiedon;
            _ModifiedBy = modifiedby;
            _IsEditable = iseditable;
        }

        public UserExt(int sequenceno, long id, string username, string password, string name, long roleid, System.DateTime modifiedon, string modifiedby, bool iseditable)
        {
            _SequenceNumber = sequenceno;
            _ID = id;
            _Username = username;
            _Password = password;
            _Name = name;
            _RoleID = roleid;
            _ModifiedOn = modifiedon;
            _ModifiedBy = modifiedby;
            _IsEditable = iseditable;
        }

        #endregion Ctor
    }
}
