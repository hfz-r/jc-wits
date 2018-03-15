using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;

namespace DataLayer.Repositories
{
    public class ModuleAccessCtrlRepository : IModuleAccessCtrlRepository
    {
        public IEnumerable<ModuleAccessCtrl> GetAll()
        {
            using (var context = new InventoryContext())
            {
                return context.ModuleAccessCtrls.OrderBy(x => x.ID).ToList();
            }
        }

        public ModuleAccessCtrl GetModuleAccessCtrl(long ID)
        {
            using (var context = new InventoryContext())
            {
                var module = context.ModuleAccessCtrls.Find(ID);
                return module;
            }
        }

        public ModuleAccessCtrl GetModuleByModuleName(string ModuleName)
        {
            using (var context = new InventoryContext())
            {
                var module = context.ModuleAccessCtrls.Where(m => m.Module == ModuleName).FirstOrDefault();
                if (module != null)
                {
                    context.Entry(module).Collection(m => m.ModuleAccessCtrlTransactions).Load();
                }
                return module;
            }
        }
    }
}
