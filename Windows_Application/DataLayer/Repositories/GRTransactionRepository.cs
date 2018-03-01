using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DataLayer.Repositories
{
    public class GRTransactionRepository : IGRTransactionRepository
    {
        public IEnumerable<GRTransaction> GetAll(bool eagerLoading)
        {
            using (var context = new InventoryContext())
            {
                if (eagerLoading)
                    return context.GRTransactions.Include(g => g.GoodsReceive).Include(rs => rs.Reason).ToList();
                else
                    return context.GRTransactions.ToList();
            }
        }

        public IEnumerable<GRTransaction> GetGRTransactionByGRID(long GRID)
        {
            using (var context = new InventoryContext())
            {
                var getAll = GetAll(true);
                return getAll.Where(x => x.GRID == GRID);
            }
        }
    }
}
