using System.Collections.Generic;
using DataLayer;

namespace ESD.JC_RoleMgmt.Services
{
    public interface IModuleAccessCtrlTransactionServices
    {
        IEnumerable<ModuleAccessCtrlTransaction> GetAll();
        List<ModuleAccessCtrlTransaction> GetModuleAccessCtrlTransaction(long ID);
        bool Save(ModuleAccessCtrlTransaction module, string state = "");
    }
}