using ESD.JC_RoleMgmt.Utilities;
using System;

namespace ESD.JC_RoleMgmt.ModelsExt
{
    public class ModuleAccessCtrlExt : ObservableObject, ISequencedObject
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

        private string _Module;
        public string Module
        {
            get { return _Module; }
            set
            {
                _Module = value;
                base.RaisePropertyChangedEvent("Module");
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

        public ModuleAccessCtrlExt()
        {
        }


        public ModuleAccessCtrlExt(long id, string module, bool isChecked)
        {
            _ID = id;
            _Module = module;
            _IsChecked = isChecked;
        }

        #endregion
    }
}
