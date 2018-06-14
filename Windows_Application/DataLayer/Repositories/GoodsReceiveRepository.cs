using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System;

namespace DataLayer.Repositories
{
    public class GoodsReceiveRepository : IGoodsReceiveRepository
    {
        public IEnumerable<GoodsReceive> GetAll(bool eagerLoading)
        {
            using (var context = new InventoryContext())
            {
                if (eagerLoading)
                    return context.GoodsReceives.Include(t => t.GRTransactions).ToList();
                else
                    return context.GoodsReceives.ToList();
            }
        }

        public GoodsReceive GetGR(long ID)
        {
            using (var context = new InventoryContext())
            {
                var gr = context.GoodsReceives.Find(ID);
                if (gr != null)
                {
                    context.Entry(gr).Collection(x => x.GRTransactions).Load();
                }
                return gr;
            }
        }

        public GoodsReceive GetGRBySAPNo(string sap_no, string po)
        {
            using (var context = new InventoryContext())
            {
                var gr = context.GoodsReceives.Where(x => x.Material == sap_no && x.PurchaseOrder == po).FirstOrDefault();
                if (gr != null)
                {
                    context.Entry(gr).Collection(x => x.GRTransactions).Load();
                }
                return gr;
            }
        }

        public GoodsReceive GetEunKGDetails(long ID)
        {
            using (var context = new InventoryContext())
            {
                var gr = context.GoodsReceives.Find(ID);
                if (gr != null)
                {
                    context.Entry(gr).Collection(x => x.EunKGs).Load();
                }
                return gr;
            }
        }

        public void Add(GoodsReceive gr)
        {
            using (var context = new InventoryContext())
            {
                context.GoodsReceives.Add(gr);
                context.SaveChanges();
            }
        }

        public void Update(GoodsReceive gr)
        {
            using (var context = new InventoryContext())
            {
                context.Entry(gr).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public void Delete(long ID)
        { using (var context = new InventoryContext())
            {

                var GITxn = context.GITransactions.Where(id => id.GRID == ID);
                if (GITxn.Any())
                    context.GITransactions.RemoveRange(GITxn);

                var GRTxn = context.GRTransactions.Where(id => id.GRID == ID);
                if (GRTxn.Any())
                    context.GRTransactions.RemoveRange(GRTxn);

                var EUNTxn = context.EunKGs.Where(id => id.GRID == ID);
                if (EUNTxn.Any())
                    context.EunKGs.RemoveRange(EUNTxn);

                var GR = context.GoodsReceives.Where(id => id.ID == ID);
                context.GoodsReceives.RemoveRange(GR);

                context.SaveChanges();
            }
        }
    }
}
