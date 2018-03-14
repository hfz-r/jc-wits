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
    }
}
