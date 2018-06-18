using System.Linq;
using System.Collections.Generic;
using System;
using System.Data.Entity;

namespace DataLayer.Repositories
{
    public class GoodsIssueRepository : IGoodsIssueRepository
    {
        public IEnumerable<GITransaction> GetAll(bool eagerLoading)
        {
            using (var context = new InventoryContext())
            {
                if (eagerLoading)
                    return context.GITransactions
                        .Include(loc => loc.Location)
                        .Include(loc1 => loc1.Location1).OrderBy(x => x.ID).ToList();
                else
                    return context.GITransactions.OrderBy(x => x.ID).ToList();
            }
        }

        //public GITransaction GetGoodsIssue(long ID)
        //{
        //    using (var context = new InventoryContext())
        //    {
        //        var gi = context.GITransactions.Find(ID);
        //        if (gi != null)
        //        {
        //            context.Entry(gi).Reference(gr => gr.GoodsReceive).Load();
        //        }
        //        return gi;
        //    }
        //}

        public GITransaction GetGI(string Material)
        {
            using (var context = new InventoryContext())
            {
                var getAll = GetAll(true);
                return getAll.Where(x => x.Material == Material).FirstOrDefault();
            }
        }

        public void Delete(string Material)
        {
            using (var context = new InventoryContext())
            {
                var GITxn = context.GITransactions.Where(id => id.Material == Material);
                if (GITxn.Any())
                    context.GITransactions.RemoveRange(GITxn);

                context.SaveChanges();
            }
        }
    }
}
