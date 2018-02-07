using System.Linq;
using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public class GoodsReceiveRepository : IGoodsReceiveRepository
    {
        public List<GoodsReceive> GetAll()
        {
            using (var context = new InventoryContext())
            {
                return context.GoodsReceives.OrderBy(x => x.PostingDate).ToList();
            }
        }

        public GoodsReceive GetGR(string purchase_order)
        {
            using (var context = new InventoryContext())
            {
                return context.GoodsReceives.Where(x => x.PurchaseOrder == purchase_order).FirstOrDefault();
            }
        }

        public void Save(GoodsReceive gr)
        {
            using (var context = new InventoryContext())
            {
                var obj = context.GoodsReceives.Where(x => x.PurchaseOrder == gr.PurchaseOrder).FirstOrDefault();

                if (obj == null)
                {
                    context.GoodsReceives.Add(gr);
                }
                else
                {
                    //edit func
                }

                context.SaveChanges();
            }
        }
    }
}
