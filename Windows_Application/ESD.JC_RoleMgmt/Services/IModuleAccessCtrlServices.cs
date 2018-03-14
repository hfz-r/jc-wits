using System.Collections.Generic;
using DataLayer;

namespace ESD.JC_RoleMgmt.Services
{
    public interface IModuleAccessCtrlServices
    {
        IEnumerable<ModuleAccessCtrl> GetAll();
        ModuleAccessCtrl GetModule(long ID);
    }
}