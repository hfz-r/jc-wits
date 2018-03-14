using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DataLayer.Repositories
{
    public class AHUTransactionRepository : IAHUTransactionRepository
    {
        public IEnumerable<AHUTransaction> GetAll(bool eagerLoading)
        {
            using (var context = new InventoryContext())
            {
                if (eagerLoading)
                    return context.AHUTransactions.Include(g => g.AHU).Include(c => c.Country).ToList();
                else
                    return context.AHUTransactions.ToList();
            }
        }

        public IEnumerable<AHUTransaction> GetAHUTransactionByAHUID(long AHUID)
        {
            using (var context = new InventoryContext())
            {
                var getAll = GetAll(true);
                return getAll.Where(x => x.AHUID == AHUID);
            }
        }
    }
}
