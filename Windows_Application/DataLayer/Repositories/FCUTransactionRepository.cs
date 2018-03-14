using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DataLayer.Repositories
{
    public class FCUTransactionRepository : IFCUTransactionRepository
    {
        public IEnumerable<FCUTransaction> GetAll(bool eagerLoading)
        {
            using (var context = new InventoryContext())
            {
                if (eagerLoading)
                    return context.FCUTransactions.Include(g => g.FCU).Include(c => c.Country).ToList();
                else
                    return context.FCUTransactions.ToList();
            }
        }

        public IEnumerable<FCUTransaction> GetFCUTransactionByFCUID(long FCUID)
        {
            using (var context = new InventoryContext())
            {
                var getAll = GetAll(true);
                return getAll.Where(x => x.FCUID == FCUID);
            }
        }
    }
}
