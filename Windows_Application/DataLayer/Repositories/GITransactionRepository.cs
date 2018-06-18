using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DataLayer.Repositories
{
    public class GITransactionRepository : IGITransactionRepository
    {
        public IEnumerable<GITransaction> GetAll(bool eagerLoading)
        {
            using (var context = new InventoryContext())
            {
                if (eagerLoading)
                    return context.GITransactions.Include(c => c.Location).Include(c => c.Location1).ToList();
                else
                    return context.GITransactions.ToList();
            }
        }

        public IEnumerable<GITransaction> GetGITransaction(long ID)
        {
            using (var context = new InventoryContext())
            {
                var getAll = GetAll(true);
                return getAll.Where(x => x.ID == ID);
            }
        }

        public void Delete(long ID)
        {
            using (var context = new InventoryContext())
            {
                var GITxn = context.GITransactions.Where(x => x.ID == ID);
                if (GITxn.Any())
                    context.GITransactions.RemoveRange(GITxn);

                context.SaveChanges();
            }
        }
    }
}
