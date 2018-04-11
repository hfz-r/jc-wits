using ESD.JC_UserMgmt.Utilities;

namespace ESD.JC_UserMgmt.ModelsExt
{
    public class RoleExt : ObservableObject
    {
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

        private string _RoleCode;
        public string RoleCode
        {
            get { return _RoleCode; }
            set
            {
                _RoleCode = value;
                base.RaisePropertyChangedEvent("RoleCode");
            }
        }
    }
}
