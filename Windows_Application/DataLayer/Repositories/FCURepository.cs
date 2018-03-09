using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;

namespace DataLayer.Repositories
{
    public class FCURepository : IFCURepository
    {
        public IEnumerable<FCU> GetAll(bool eagerLoading)
        {
            using (var context = new InventoryContext())
            {
                if (eagerLoading)
                    return context.FCUs.Include(t => t.FCUTransactions).ToList();
                else
                    return context.FCUs.ToList();
            }
        }

        public FCU GetFCU(long ID)
        {
            using (var context = new InventoryContext())
            {
                var fcu = context.FCUs.Find(ID);
                if (fcu != null)
                {
                    context.Entry(fcu).Collection(x => x.FCUTransactions).Load();
                }
                return fcu;
            }
        }

        public FCU GetFCUBySerialNo(string serial_no)
        {
            using (var context = new InventoryContext())
            {
                var fcu = context.FCUs.Where(x => x.SerialNo == serial_no).FirstOrDefault();
                if (fcu != null)
                {
                    context.Entry(fcu).Collection(x => x.FCUTransactions).Load();
                }
                return fcu;
            }
        }

        public void Add(FCU fcu)
        {
            using (var context = new InventoryContext())
            {
                context.FCUs.Add(fcu);
                context.SaveChanges();
            }
        }

        public void Update(FCU fcu)
        {
            using (var context = new InventoryContext())
            {
                context.Entry(fcu).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}
