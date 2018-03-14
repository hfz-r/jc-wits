using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;

namespace DataLayer.Repositories
{
    public class ModuleAccessCtrlTransactionRepository : IModuleAccessCtrlTransactionRepository
    {
        public IEnumerable<ModuleAccessCtrlTransaction> GetAll()
        {
            using (var context = new InventoryContext())
            {
                return context.ModuleAccessCtrlTransactions.OrderBy(x => x.ID).ToList();
            }
        }

        public List<ModuleAccessCtrlTransaction> GetModuleAccessCtrlTransaction(long RoleID)
        {
            using (var context = new InventoryContext())
            {
                return context.ModuleAccessCtrlTransactions.Where(x => x.RoleID == RoleID).ToList();
            }
        }

        public void Add(ModuleAccessCtrlTransaction txn)
        {
            using (var context = new InventoryContext())
            {
                var searchModuleAccessCtrlTransaction = context.ModuleAccessCtrlTransactions.Where(x => x.RoleID == txn.RoleID && x.ModuleID == txn.ModuleID).FirstOrDefault();
                if (searchModuleAccessCtrlTransaction == null)
                {
                    context.ModuleAccessCtrlTransactions.Add(txn);
                    context.SaveChanges();
                }
            }
        }

        public void Update(ModuleAccessCtrlTransaction txn)
        {
            using (var context = new InventoryContext())
            {

                context.Entry(txn).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}
