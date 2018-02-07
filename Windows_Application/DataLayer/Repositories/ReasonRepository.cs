using System.Linq;
using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public class ReasonRepository : IReasonRepository
    {
        public List<Reason> GetAll()
        {
            using (var context = new InventoryContext())
            {
                return context.Reasons.OrderBy(x => x.ID).ToList();
            }
        }

        public Reason GetReason(long ID)
        {
            using (var context = new InventoryContext())
            {
                var reasons = GetAll();

                return reasons.Where(x => x.ID == ID).FirstOrDefault();
            }
        }

        public void Save(Reason reason)
        {
            using (var context = new InventoryContext())
            {
                var obj = context.Reasons.Where(x => x.ID == reason.ID).FirstOrDefault();

                if (obj == null)
                {
                    context.Reasons.Add(reason);
                }
                else
                {
                    obj.ReasonDesc = reason.ReasonDesc;
                    obj.ModifiedOn = reason.ModifiedOn;
                    obj.ModifiedBy = reason.ModifiedBy;
                }

                context.SaveChanges();
            }
        }

        public void Delete(long ID)
        {
            using (var context = new InventoryContext())
            {
                var obj = context.Reasons.Where(x => x.ID == ID).FirstOrDefault();
                if (obj != null)
                {
                    context.Reasons.Remove(obj);
                }

                context.SaveChanges();
            }
        }

    }
}
