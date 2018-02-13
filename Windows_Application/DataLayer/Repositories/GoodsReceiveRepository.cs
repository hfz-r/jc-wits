using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;

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

        public GoodsReceive GetSAPNo(string sap_no)
        {
            using (var context = new InventoryContext())
            {
                var gr = context.GoodsReceives.Where(x => x.Material == sap_no).FirstOrDefault();
                if (gr != null)
                {
                    context.Entry(gr).Collection(x => x.GRTransactions).Load();
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
    }
}
