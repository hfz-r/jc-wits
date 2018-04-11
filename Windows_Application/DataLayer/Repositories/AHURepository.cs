using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;

namespace DataLayer.Repositories
{
    public class AHURepository : IAHURepository
    {
        public IEnumerable<AHU> GetAll(bool eagerLoading)
        {
            using (var context = new InventoryContext())
            {
                if (eagerLoading)
                    return context.AHUs.Include(t => t.AHUTransactions).ToList();
                else
                    return context.AHUs.ToList();
            }
        }

        public AHU GetAHU(long ID)
        {
            using (var context = new InventoryContext())
            {
                var ahu = context.AHUs.Find(ID);
                if (ahu != null)
                {
                    context.Entry(ahu).Collection(x => x.AHUTransactions).Load();
                }
                return ahu;
            }
        }

        public AHU GetAHUBySerialNo(string serial_no)
        {
            using (var context = new InventoryContext())
            {
                var ahu = context.AHUs.Where(x => x.SerialNo == serial_no).FirstOrDefault();
                if (ahu != null)
                {
                    context.Entry(ahu).Collection(x => x.AHUTransactions).Load();
                }
                return ahu;
            }
        }

        public void Add(AHU ahu)
        {
            using (var context = new InventoryContext())
            {
                context.AHUs.Add(ahu);
                context.SaveChanges();
            }
        }

        public void Update(AHU ahu)
        {
            using (var context = new InventoryContext())
            {
                context.Entry(ahu).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public void Delete(long ID)
        {
            using (var context = new InventoryContext())
            {
                var ahuTxn = context.AHUTransactions.Where(id => id.AHUID == ID);
                context.AHUTransactions.RemoveRange(ahuTxn);

                var ahu = context.AHUs.Where(id => id.ID == ID);
                context.AHUs.RemoveRange(ahu);

                context.SaveChanges();
            }
        }
    }
}
