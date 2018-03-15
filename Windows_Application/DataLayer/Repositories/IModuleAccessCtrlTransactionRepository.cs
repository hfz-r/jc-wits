using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public interface IModuleAccessCtrlTransactionRepository
    {
        IEnumerable<ModuleAccessCtrlTransaction> GetAll();
        IEnumerable<ModuleAccessCtrlTransaction> GetAll(bool eagerLoading);
        List<ModuleAccessCtrlTransaction> GetModuleAccessCtrlTransaction(long ID);
        void Add(ModuleAccessCtrlTransaction reason);
        void Update(ModuleAccessCtrlTransaction reason);
    }
}