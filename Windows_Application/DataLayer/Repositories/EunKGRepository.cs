using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;

namespace DataLayer.Repositories
{
    public class EunKGRepository : IEunKGRepository
    {
        public IEnumerable<EunKG> GetAll(bool eagerLoading)
        {
            using (var context = new InventoryContext())
            {
                if (eagerLoading)
                    return context.EunKGs.Include(t => t.GoodsReceive).ToList();
                else
                    return context.EunKGs.ToList();
            }
        }

        public EunKG GetEunKG(long ID)
        {
            using (var context = new InventoryContext())
            {
                var kg = context.EunKGs.Find(ID);
                if (kg != null)
                {
                    context.Entry(kg).Reference(x => x.GoodsReceive).Load();
                }
                return kg;
            }
        }

        public void Add(EunKG kg)
        {
            using (var context = new InventoryContext())
            {
                context.EunKGs.Add(kg);
                context.SaveChanges();
            }
        }

        public void Update(EunKG kg)
        {
            using (var context = new InventoryContext())
            {
                context.Entry(kg).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public void Delete(long ID)
        {
            using (var context = new InventoryContext())
            {
                var reasons = context.EunKGs.Where(id => id.ID == ID).Include(gr => gr.GoodsReceive);
                context.EunKGs.RemoveRange(reasons);

                context.SaveChanges();
            }
        }
    }
}
