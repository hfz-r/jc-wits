using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public interface IModuleAccessCtrlRepository
    {
        IEnumerable<ModuleAccessCtrl> GetAll();
        ModuleAccessCtrl GetModuleAccessCtrl(long ID);
        ModuleAccessCtrl GetModuleByModuleName(string ModuleName);
    }
}